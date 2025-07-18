using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.Themes.Fluent;
using DefaultApplication.Internal;
using DefaultApplication.Internal.Plugins.ShellPlugin.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Extensions.Logging;

namespace DefaultApplication;

public class DesktopRuner : BaseRuner
{
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

    protected override Task<AppBuilder> ConfigureBuilderAsync(AppBuilder builder)
        => Task.FromResult(
            builder
                .UsePlatformDetect()
                .LogToTrace()
                .SetupWithoutStarting());

    protected override Application CreateApplication()
    {
        Application application = new();

        application.Styles.Add(new FluentTheme());

        return application;
    }

    protected override ISplashScreen CreateSplashScreen(Application application, Microsoft.Extensions.Logging.ILogger logger) => new DefaultSplashScreen(logger);

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

    protected override TopLevel CreateMainTopLevel(Application application)
    {
        using Stream iconStream = AssetLoader.Open(new Uri("avares://DefaultApplication.Core/Resources/Images/DefaultLogo.png"));

        Window window = new()
        {
            ExtendClientAreaToDecorationsHint = true,
            Icon = new WindowIcon(iconStream),
            Title = "Default Application",
            WindowState = WindowState.Maximized
        };

#if DEBUG
        window.AttachDevTools();
#endif

        return window;
    }
}