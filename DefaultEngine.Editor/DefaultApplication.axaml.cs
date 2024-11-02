using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace DefaultEngine;

public class DefaultApplication : Application
{
    private static int Main(string[] args)
    {
        return AppBuilder
            .Configure<DefaultApplication>()
            .UsePlatformDetect()
            .StartWithClassicDesktopLifetime(args);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime applicationLifetime)
        {
            applicationLifetime.MainWindow = new DefaultSplashScreen(applicationLifetime);
        }

        base.OnFrameworkInitializationCompleted();
    }
}