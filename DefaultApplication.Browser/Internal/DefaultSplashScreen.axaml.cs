using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using Microsoft.Extensions.Logging;

namespace DefaultApplication.Internal;

internal sealed partial class DefaultSplashScreen : DockPanel, ISplashScreen
{
    private readonly ILogger _logger;

    public DefaultSplashScreen(ILogger logger)
    {
        InitializeComponent();

        _logger = logger;
    }

    [LoggerMessage(LogLevel.Information, "{Message}")]
    private static partial void LogMessage(ILogger logger, string message);

    public async Task ReportAsync(string message)
    {
        LogMessage(_logger, message);
        await Dispatcher.UIThread.InvokeAsync(() => InformationsTextBlock.Text = message);
    }

    public void Dispose()
    { }
}