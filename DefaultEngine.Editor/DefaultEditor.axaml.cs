using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using DefaultEngine.Editor.Services;
using DefaultEngine.Editor.ViewModels;
using DefaultUnDo;
using Microsoft.Extensions.DependencyInjection;

namespace DefaultEngine.Editor;

public class DefaultEditor : Application
{
    private static int Main(string[] args) => StartEditor<DefaultEditor>(args);

    private static void Register(IServiceCollection services)
    {
        services.AddSingleton<ShellViewModel>();
        services.AddSingleton<IUnDoManager, UnDoManager>();
        services.AddSingleton<PluginsService>();
    }

    private static Task<ShellViewModel> CreateContentAsync(Action<string> notifier) => Task.Run(() =>
    {
        notifier("registering services");

        ServiceCollection services = new();

        Register(services);

        notifier("building content");

        return services.BuildServiceProvider().GetRequiredService<ShellViewModel>();
    });

    public static int StartEditor<T>(string[] args)
        where T : DefaultEditor
        => AppBuilder
            .Configure<DefaultEditor>()
            .UsePlatformDetect()
            .StartWithClassicDesktopLifetime(args);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime applicationLifetime)
        {
            applicationLifetime.MainWindow = new DefaultSplashScreen(applicationLifetime, CreateContentAsync);
        }

        base.OnFrameworkInitializationCompleted();
    }
}