using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using DefaultEngine.ViewModels;
using DefaultUnDo;
using Microsoft.Extensions.DependencyInjection;

namespace DefaultEngine;

public partial class DefaultSplashScreen : Window
{
    private readonly IClassicDesktopStyleApplicationLifetime _applicationLifetime;

    public DefaultSplashScreen(IClassicDesktopStyleApplicationLifetime applicationLifetime)
    {
        _applicationLifetime = applicationLifetime;

        InitializeComponent();
    }

    private static void Register(IServiceCollection services)
    {
        services.AddSingleton<ShellViewModel>();
        services.AddSingleton<IUnDoManager, UnDoManager>();
    }

    private static Task<ShellViewModel> CreateContentAsync() => Task.Run(() =>
    {
        ServiceCollection services = new();

        Register(services);

        return services.BuildServiceProvider().GetRequiredService<ShellViewModel>();
    });

    protected override async void OnInitialized()
    {
        base.OnInitialized();

        await Task.Yield();

        _applicationLifetime.MainWindow = new Window
        {
            Title = "Default Engine",
            WindowState = WindowState.Maximized,
            Icon = Icon,
            Content = await CreateContentAsync().ConfigureAwait(true)
        };

        _applicationLifetime.MainWindow.Show();

#if DEBUG
        _applicationLifetime.MainWindow.AttachDevTools();
#endif

        InformationsTextBlock.Text = "wellcome";

        await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(true);

        Close();
    }

    private void OnClicked(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Visual visual
            && e.GetCurrentPoint(visual).Properties.IsLeftButtonPressed)
        {
            Hide();
        }
    }
}