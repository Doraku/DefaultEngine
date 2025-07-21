using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Themes.Fluent;
using DefaultApplication.Internal;
using DefaultApplication.Internal.Plugins.ShellPlugin.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Extensions.Logging;

namespace DefaultApplication;

internal sealed class BrowserApplication : Application
{
    public event Action? ShutdownRequested;

    public void RequestShutdown() => ShutdownRequested?.Invoke();
}

internal sealed class BrowserRuner : BaseRuner
{
    private static async Task Main(string[] args)
    {
        using BrowserRuner runner = new();

        await runner.RunAsync(args).ConfigureAwait(false);
    }

    protected override Microsoft.Extensions.Logging.ILogger CreateLogger()
    {
        Trace.Listeners.Add(new ConsoleTraceListener());

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Debug(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}",
                formatProvider: CultureInfo.InvariantCulture)
            .CreateLogger();

        return new SerilogLoggerProvider().CreateLogger("DefaultApplication");
    }

    protected override Application CreateApplication()
    {
        BrowserApplication application = new();

        application.Styles.Add(new FluentTheme());

        return application;
    }

    protected override async Task<AppBuilder> ConfigureBuilderAsync(AppBuilder builder)
    {
        builder = builder
            .WithInterFont()
            .LogToTrace();

        await builder.StartBrowserAppAsync("out").ConfigureAwait(false);

        return builder;
    }

    protected override async Task RunAsync(AppBuilder builder, CancellationToken cancellationToken)
    {
        TaskCompletionSource task = new();

        using (cancellationToken.Register(task.SetResult))
        {
            await task.Task.ConfigureAwait(false);
        }
    }

    protected override ISplashScreen CreateSplashScreen(Application application, Microsoft.Extensions.Logging.ILogger logger)
    {
        DefaultSplashScreen splashScreen = new(logger);

        if (application.ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = splashScreen;
        }

        return splashScreen;
    }

    protected override IServiceProvider BuildServiceProvider(IServiceCollection services)
        => services
            .AddLogging(builder => builder.AddSerilog(dispose: true))
            .BuildServiceProvider(
                new ServiceProviderOptions
                {
                    ValidateOnBuild = true,
                    ValidateScopes = true
                });

    protected override async Task<object> CreateContentAsync(IServiceProvider services)
    {
        await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);

        return services.GetRequiredService<ShellViewModel>();
    }

    protected override TopLevel CreateMainTopLevel(Application application, CancellationTokenSource shutdownTokenSource)
    {
        if (application is BrowserApplication browserApplication)
        {
            browserApplication.ShutdownRequested += shutdownTokenSource.Cancel;
        }

        TopLevel? topLevel = null;

        if (application.ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            topLevel = TopLevel.GetTopLevel(singleViewPlatform.MainView);
        }

        return topLevel ?? throw new Exception("Could not retrieve TopLevel");
    }
}