using System;
using System.Collections.Generic;
using System.IO;
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

    private async Task<IServiceProvider> CreatePluginsProviderAsync(Application application)
    {
        await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);

        PluginsHelper plugins = new(new FileInfo(Assembly.GetEntryAssembly()!.Location).Directory!);

        ServiceCollection services = [];

        services
            .AddSingleton(plugins)
            .AddSingleton(application);

        foreach (Type type in plugins.GetPluginsTypes().GetInstanciableImplementation<IServicesRegisterer>())
        {
            services.AddAsSingletonImplementation<IServicesRegisterer>(type);
        }

        IServiceProvider provider = BuildServiceProvider(services);

        if (provider is IDisposable disposable)
        {
            _disposables.Add(disposable);
        }

        return provider;
    }

    private async Task<(IServiceProvider, TaskCompletionSource<TopLevel>)> CreateServicesAsync(Application application, IServiceProvider pluginsProvider)
    {
        await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);

        ServiceCollection services = [];

        services.AddSingleton(application);

        foreach (IServicesRegisterer servicesRegisterer in pluginsProvider.GetRequiredService<IEnumerable<IServicesRegisterer>>())
        {
            services.AddAsSingletonImplementation(servicesRegisterer);

            servicesRegisterer.Register(services);
        }

        services.AddDelayedSingleton(out TaskCompletionSource<TopLevel> delayedMainTopLevel);

        IServiceProvider provider = BuildServiceProvider(services);

        if (provider is IDisposable disposable)
        {
            _disposables.Add(disposable);
        }

        provider.GetService<IEnumerable<IPlugin>>();

        return (provider, delayedMainTopLevel);
    }

    public interface ISplashScreen : IDisposable
    {
        Task ReportAsync(string message);
    }

    private async Task InitializeAsync(ILogger logger, Application application, CancellationTokenSource shutdownTokenSource)
    {
        try
        {
            using ISplashScreen splashScreen = CreateSplashScreen(logger);

            await splashScreen.ReportAsync("registering plugins").ConfigureAwait(true);

            IServiceProvider plugins = await CreatePluginsProviderAsync(application).ConfigureAwait(true);

            await splashScreen.ReportAsync("registering services").ConfigureAwait(true);

            (IServiceProvider services, TaskCompletionSource<TopLevel> delayedMainTopLevel) = await CreateServicesAsync(application, plugins).ConfigureAwait(true);

            await splashScreen.ReportAsync("creating content").ConfigureAwait(true);

            object content = await CreateContentAsync(services).ConfigureAwait(true);

            await splashScreen.ReportAsync("creating main top level").ConfigureAwait(true);

            await Task.Delay(TimeSpan.FromSeconds(0.5)).ConfigureAwait(true);

            TopLevel topLevel = CreateMainTopLevel();

            await splashScreen.ReportAsync("welcome").ConfigureAwait(true);

            topLevel.Content = content;
            topLevel.Closed += (_, _) => shutdownTokenSource.Cancel();

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
        catch (Exception exception)
        {
            logger.LogInitializationException(exception);

            await shutdownTokenSource.CancelAsync().ConfigureAwait(true);

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

    public void Run(string[] args)
    {
        void Run(Application app, string[] args)
        {
            ILogger logger = CreateLogger();

            AppDomain.CurrentDomain.UnhandledException += (_, args) => logger.LogUnhandledException(args.ExceptionObject as Exception);
            TaskScheduler.UnobservedTaskException += (_, args) => logger.LogUnobservedTaskException(args.Exception);

            logger.LogStart(args);

            using CancellationTokenSource shutdownTokenSource = new();

            _ = InitializeAsync(logger, app, shutdownTokenSource);

            app.Run(shutdownTokenSource.Token);
        }

        ConfigureBuilder(AppBuilder.Configure(CreateApplication)).Start(Run, args);
    }

    #region IDisposable

    private bool _disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _disposables.Reverse();

                foreach (IDisposable disposable in _disposables)
                {
                    disposable.Dispose();
                }
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}