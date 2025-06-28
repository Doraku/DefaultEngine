using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DefaultApplication.Internal;
using DefaultApplication.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DefaultApplication;

public abstract class BaseRuner : IDisposable
{
    private readonly List<IDisposable> _disposables;

    protected BaseRuner()
    {
        _disposables = [];
    }

    private async Task<IServiceProvider> CreateServiceRegisterersAsync(Application? application)
    {
        await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);

        PluginsHelper plugins = new(new FileInfo(Assembly.GetEntryAssembly()!.Location).Directory!);

        ServiceCollection services = [];

        services.AddSingleton(plugins);

        if (application is { })
        {
            services.AddSingleton(application);
        }

        foreach (Type type in plugins.GetPluginsTypes().GetInstanciableImplementation<IServiceRegisterer>())
        {
            services.AddAsSingletonImplementation<IServiceRegisterer>(type);
        }

        IServiceProvider provider = BuildServiceProvider(services);

        if (provider is IDisposable disposable)
        {
            _disposables.Add(disposable);
        }

        return provider;
    }

    private async Task<(IServiceProvider, TaskCompletionSource<TopLevel>?)> CreateServicesAsync(Application? application, IServiceProvider serviceRegisterers)
    {
        await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);

        ServiceCollection services = [];
        TaskCompletionSource<TopLevel>? delayedMainTopLevel = null;

        if (application is { })
        {
            services.AddSingleton(application);
            services.AddDelayedSingleton(out delayedMainTopLevel);
        }

        foreach (IServiceRegisterer servicesRegisterer in serviceRegisterers.GetRequiredService<IEnumerable<IServiceRegisterer>>())
        {
            services.AddAsSingletonImplementation(servicesRegisterer);

            servicesRegisterer.Register(services);
        }

        IServiceProvider provider = BuildServiceProvider(services);

        if (provider is IDisposable disposable)
        {
            _disposables.Add(disposable);
        }

        return (provider, delayedMainTopLevel);
    }

    private async Task InitializeAsync(ILogger logger, Application? application, CancellationTokenSource? shutdownTokenSource)
    {
        try
        {
            IEnumerable<IPlugin>? plugins;

            using (ISplashScreen splashScreen = application is { } ? CreateSplashScreen(logger) : new NoApplicationSplashScreen(logger))
            {
                await splashScreen.ReportAsync("registering service registerers").ConfigureAwait(true);

                IServiceProvider serviceRegistererProvider = await CreateServiceRegisterersAsync(application).ConfigureAwait(true);

                await splashScreen.ReportAsync("registering services").ConfigureAwait(true);

                (IServiceProvider services, TaskCompletionSource<TopLevel>? delayedMainTopLevel) = await CreateServicesAsync(application, serviceRegistererProvider).ConfigureAwait(true);

                await splashScreen.ReportAsync("creating plugins").ConfigureAwait(true);

                plugins = await Task.Run(() => services.GetService<IEnumerable<IPlugin>>()).ConfigureAwait(true) ?? [];

                await splashScreen.ReportAsync("creating content").ConfigureAwait(true);

                object content = await CreateContentAsync(services).ConfigureAwait(true);

                if (delayedMainTopLevel is { })
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.5)).ConfigureAwait(true);

                    TopLevel topLevel = CreateMainTopLevel();

                    await splashScreen.ReportAsync("hello").ConfigureAwait(true);

                    topLevel.Content = content;
                    topLevel.Closed += (_, _) => shutdownTokenSource?.Cancel();

                    switch (topLevel)
                    {
                        case WindowBase window:
                            window.Show();
                            break;

                        default:
                            throw new NotSupportedException($"Unhandled TopLevel type {topLevel.GetType()}");
                    }

                    delayedMainTopLevel.SetResult(topLevel);

                    await Task.Delay(TimeSpan.FromSeconds(0.5)).ConfigureAwait(true);
                }
            }

            await Task.WhenAll(plugins.Select(plugin => plugin.StartAsync())).ConfigureAwait(true);
        }
        catch (Exception exception)
        {
            logger.LogInitializationException(exception);

            await (shutdownTokenSource?.CancelAsync() ?? Task.CompletedTask).ConfigureAwait(true);

            throw;
        }
    }

    protected abstract ILogger CreateLogger();

    protected abstract AppBuilder ConfigureBuilder(AppBuilder builder);

    protected abstract Application CreateApplication();

    protected abstract ISplashScreen CreateSplashScreen(ILogger logger);

    protected abstract IServiceProvider BuildServiceProvider(IServiceCollection services);

    protected abstract Task<object> CreateContentAsync(IServiceProvider services);

    protected abstract TopLevel CreateMainTopLevel();

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types")]
    public Task RunAsync(string[] args)
    {
        TaskCompletionSource taskCompletionSource = new();

        ILogger logger = CreateLogger();

        AppDomain.CurrentDomain.UnhandledException += (_, args) => logger.LogUnhandledException(args.ExceptionObject as Exception);
        TaskScheduler.UnobservedTaskException += (_, args) => logger.LogUnobservedTaskException(args.Exception);

        logger.LogStart(args);

        void Run(Application app, string[] _)
        {
            using CancellationTokenSource shutdownTokenSource = new();

            Task initializationTask = InitializeAsync(logger, app, shutdownTokenSource);

            try
            {
                app.Run(shutdownTokenSource.Token);
                initializationTask.ContinueWith(_ => taskCompletionSource.SetResult(), TaskScheduler.Default);
            }
            catch (Exception exception)
            {
                logger.LogRunnerException(exception);
                taskCompletionSource.SetException(exception);
            }
        }

        new Thread(() => ConfigureBuilder(AppBuilder.Configure(CreateApplication)).Start(Run, args)).Start();

        return taskCompletionSource.Task;
    }

    #region IDisposable

    private bool _disposedValue;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types")]
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            _disposedValue = true;

            if (disposing)
            {
                List<Exception> innerExceptions = [];

                _disposables.Reverse();

                foreach (IDisposable disposable in _disposables)
                {
                    try
                    {
                        disposable.Dispose();
                    }
                    catch (Exception exception)
                    {
                        innerExceptions.Add(exception);
                    }
                }

                if (innerExceptions.Count > 0)
                {
                    throw new AggregateException(innerExceptions);
                }
            }
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}