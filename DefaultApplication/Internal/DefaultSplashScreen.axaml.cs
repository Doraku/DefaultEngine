using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Microsoft.Extensions.Logging;

namespace DefaultApplication.Internal;

internal sealed partial class DefaultSplashScreen : Window, ISplashScreen
{
    private readonly ILogger _logger;

    public DefaultSplashScreen(ILogger logger)
    {
        InitializeComponent();

        _logger = logger;

        Show();
    }

    [LoggerMessage(LogLevel.Information, "{Message}")]
    private static partial void LogMessage(ILogger logger, string message);

    private void OnClicked(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Visual visual
            && e.GetCurrentPoint(visual).Properties.IsLeftButtonPressed)
        {
            Hide();
        }
    }

    public async Task ReportAsync(string message)
    {
        LogMessage(_logger, message);
        await Dispatcher.UIThread.InvokeAsync(() => InformationsTextBlock.Text = message);
    }

    public void Dispose() => Close();
}