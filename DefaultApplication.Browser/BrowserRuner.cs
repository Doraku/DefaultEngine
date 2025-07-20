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

internal sealed class BrowserRuner : BaseRuner
{
    private static async Task Main(string[] args)
    {
        using BrowserRuner runner = new();

        await runner.RunAsync(args).ConfigureAwait(false);
    }

    protected override Microsoft.Extensions.Logging.ILogger CreateLogger()
    {
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
        Application application = new();

        application.Styles.Add(new FluentTheme());

        return application;
    }

    protected override async Task<AppBuilder> ConfigureBuilderAsync(AppBuilder builder)
    {
        Trace.Listeners.Add(new ConsoleTraceListener());

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

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Trimming", "IL2075:'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.", Justification = "no trimming")]
    protected override TopLevel CreateMainTopLevel(Application application)
    {
        // it should be an Avalonia.BrowserSingleViewLifetime
        return (TopLevel)application.ApplicationLifetime!.GetType().GetProperty("TopLevel")?.GetValue(application.ApplicationLifetime)!;
    }
}