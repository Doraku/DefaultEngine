using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Avalonia.Controls;
using Avalonia.Themes.Fluent;
using DefaultApplication;
using DefaultApplication.Browser;
using DefaultApplication.Browser.ViewModels;
using DefaultApplication.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Extensions.Logging;

internal sealed class Program
{
    //private static Task Main(string[] args) => BuildAvaloniaApp()
    //        .WithInterFont()
    //        .StartBrowserAppAsync("out");

    private static async Task Main(string[] args)
    {
        using BrowserRuner runner = new();

        await runner.RunAsync(args).ConfigureAwait(false);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder
            .Configure<App>()
            .LogToTrace();
}

public class BrowserRuner : BaseRuner
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

    protected override AppBuilder ConfigureBuilder(AppBuilder builder)
    {
        builder = builder
            .WithInterFont()
            .LogToTrace();

        return ((Task<AppBuilder>)typeof(BrowserAppBuilder).GetMethod("PreSetupBrowser", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, [builder, null])!).GetAwaiter().GetResult();
    }

    protected override Application CreateApplication()
    {
        Application application = new();

        application.Styles.Add(new FluentTheme());

        return application;
    }

    protected override ISplashScreen CreateSplashScreen(Microsoft.Extensions.Logging.ILogger logger) => throw new Exception();

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

        return services.GetRequiredService<MainViewModel>();
    }

    protected override TopLevel CreateMainTopLevel()
    {
        return (TopLevel)typeof(AvaloniaView).GetProperty("TopLevel", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(new AvaloniaView("out"))!;
    }
}

public class Plugin : IServicesRegisterer
{
    public void Register(IServiceCollection services) => services.AddSingleton<MainViewModel>();
}