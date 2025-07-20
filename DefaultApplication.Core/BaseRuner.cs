using System;
using System.Collections.Generic;
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

        PluginsHelper plugins = new();

        ServiceCollection services = [];

        services.AddSingleton(plugins);

        if (application is { })
        {
            services.AddSingleton(application);
        }

        foreach (Type type in plugins.GetTypes().GetInstanciableImplementation<IServiceRegisterer>())
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

            using (ISplashScreen splashScreen = application is { } ? CreateSplashScreen(application, logger) : new NoApplicationSplashScreen(logger))
            {
                await splashScreen.ReportAsync("registering service registerers").ConfigureAwait(true);

                IServiceProvider serviceRegistererProvider = await CreateServiceRegisterersAsync(application).ConfigureAwait(true);

                await splashScreen.ReportAsync("registering services").ConfigureAwait(true);

                (IServiceProvider services, TaskCompletionSource<TopLevel>? delayedMainTopLevel) = await CreateServicesAsync(application, serviceRegistererProvider).ConfigureAwait(true);

                await splashScreen.ReportAsync("creating plugins").ConfigureAwait(true);

                plugins = await Task.Run(() => services.GetService<IEnumerable<IPlugin>>()).ConfigureAwait(true) ?? [];

                await splashScreen.ReportAsync("creating content").ConfigureAwait(true);

                object content = await CreateContentAsync(services).ConfigureAwait(true);

                if (application is { } && delayedMainTopLevel is { } && shutdownTokenSource is { })
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.5)).ConfigureAwait(true);

                    TopLevel topLevel = CreateMainTopLevel(application, shutdownTokenSource);

                    await splashScreen.ReportAsync("hello").ConfigureAwait(true);

                    topLevel.Content = content;

                    delayedMainTopLevel.SetResult(topLevel);

                    if (topLevel is Window window)
                    {
                        window.Show();
                    }

                    await Task.Delay(TimeSpan.FromSeconds(0.5)).ConfigureAwait(true);
                }
            }

            await Task.WhenAll(plugins.Select(plugin => plugin.StartAsync())).ConfigureAwait(true);
        }
        catch
        {
            await (shutdownTokenSource?.CancelAsync() ?? Task.CompletedTask).ConfigureAwait(true);

            throw;
        }
    }

    protected abstract ILogger CreateLogger();

    protected abstract Application CreateApplication();

    protected abstract Task<AppBuilder> ConfigureBuilderAsync(AppBuilder builder);

    protected abstract Task RunAsync(AppBuilder builder, CancellationToken cancellationToken);

    protected abstract ISplashScreen CreateSplashScreen(Application application, ILogger logger);

    protected abstract IServiceProvider BuildServiceProvider(IServiceCollection services);

    protected abstract Task<object> CreateContentAsync(IServiceProvider services);

    protected abstract TopLevel CreateMainTopLevel(Application application, CancellationTokenSource shutdownTokenSource);

    public async Task RunAsync(string[] args)
    {
        ILogger logger = CreateLogger();

        AppDomain.CurrentDomain.UnhandledException += (_, args) => logger.LogUnhandledException(args.ExceptionObject as Exception);
        TaskScheduler.UnobservedTaskException += (_, args) => logger.LogUnobservedTaskException(args.Exception);

        logger.LogStart(args);

        try
        {
            AppBuilder builder = await ConfigureBuilderAsync(AppBuilder.Configure(CreateApplication)).ConfigureAwait(true);

            using CancellationTokenSource shutdownTokenSource = new();

            Task initializationTask = InitializeAsync(logger, builder.Instance, shutdownTokenSource);

            await RunAsync(builder, shutdownTokenSource.Token).ConfigureAwait(false);

            await initializationTask.ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            logger.LogRunnerException(exception);

            throw;
        }
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