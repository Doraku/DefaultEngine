using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DefaultApplication.Internal;

internal sealed partial class NoApplicationSplashScreen : ISplashScreen
{
    private readonly ILogger _logger;

    public NoApplicationSplashScreen(ILogger logger)
    {
        _logger = logger;
    }

    [LoggerMessage(LogLevel.Information, "{Message}")]
    private static partial void LogMessage(ILogger logger, string message);

    public Task ReportAsync(string message)
    {
        LogMessage(_logger, message);
        return Task.CompletedTask;
    }

    public void Dispose()
    { }
}
