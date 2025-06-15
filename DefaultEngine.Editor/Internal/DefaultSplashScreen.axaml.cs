using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Microsoft.Extensions.Logging;

namespace DefaultEngine.Editor.Internal;

internal sealed partial class DefaultSplashScreen : Window
{
    private readonly ILogger _logger;

    public DefaultSplashScreen(ILogger logger)
    {
        InitializeComponent();

        _logger = logger;
    }

    private void OnClicked(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Visual visual
            && e.GetCurrentPoint(visual).Properties.IsLeftButtonPressed)
        {
            Hide();
        }
    }

    public async Task SetInformations(string value)
    {
        _logger.LogInformation(value);
        await Dispatcher.UIThread.InvokeAsync(() => InformationsTextBlock.Text = value);
    }
}