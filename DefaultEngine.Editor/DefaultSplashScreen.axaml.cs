using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Threading;
using DefaultEngine.Editor.ViewModels;

namespace DefaultEngine.Editor;

internal sealed partial class DefaultSplashScreen : Window
{
    private readonly IClassicDesktopStyleApplicationLifetime _applicationLifetime;
    private readonly Func<Action<string>, Task<ShellViewModel>> _contentCreator;

    public DefaultSplashScreen(IClassicDesktopStyleApplicationLifetime applicationLifetime, Func<Action<string>, Task<ShellViewModel>> contentCreator)
    {
        _applicationLifetime = applicationLifetime;
        _contentCreator = contentCreator;

        InitializeComponent();
    }

    protected override async void OnInitialized()
    {
        base.OnInitialized();

        await Task.Yield();

        _applicationLifetime.MainWindow = new Window
        {
            Title = "Default Engine",
            WindowState = WindowState.Maximized,
            Icon = Icon,
            Content = await _contentCreator(text => Dispatcher.UIThread.Invoke(() => InformationsTextBlock.Text = text)).ConfigureAwait(true)
        };

        _applicationLifetime.MainWindow.Show();

#if DEBUG
        _applicationLifetime.MainWindow.AttachDevTools();
#endif

        InformationsTextBlock.Text = "welcome";

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