using System;
using DefaultApplication.Services;
using DefaultApplication.Settings;
using Microsoft.Extensions.Logging;

namespace DefaultApplication.Internal;

internal static partial class LoggerHelper
{
    private static readonly Action<ILogger, Exception?> _logUnhandledExceptionCallback = LoggerMessage.Define(LogLevel.Error, new EventId(0, nameof(LogUnhandledException)), "unhandled exception");

    public static void LogUnhandledException(this ILogger logger, Exception? exception) => _logUnhandledExceptionCallback(logger, exception);

    private static readonly Action<ILogger, Exception?> _logUnobservedTaskExceptionCallback = LoggerMessage.Define(LogLevel.Error, new EventId(0, nameof(LogUnhandledException)), "unobserved task exception");

    public static void LogUnobservedTaskException(this ILogger logger, Exception? exception) => _logUnobservedTaskExceptionCallback(logger, exception);

    [LoggerMessage(LogLevel.Information, "starting with args {Args}")]
    public static partial void LogStart(this ILogger logger, string[] args);

    [LoggerMessage(LogLevel.Information, "ending")]
    public static partial void LogEnd(this ILogger logger);

    private static readonly Action<ILogger, Exception?> _logRunnerException = LoggerMessage.Define(LogLevel.Critical, new EventId(0, nameof(LogRunnerException)), "runner exception");

    public static void LogRunnerException(this ILogger logger, Exception? exception) => _logRunnerException(logger, exception);

    private static readonly Action<ILogger, object?, Exception?> _logWorkerServiceExceptionCallback = LoggerMessage.Define<object?>(LogLevel.Error, new EventId(0, nameof(LogUnhandledException)), "error when running operation {OperationHeader}");

    public static void LogWorkerServiceException(this ILogger logger, IWorkerService.IOperation operation, Exception? exception) => _logWorkerServiceExceptionCallback(logger, operation.Header, exception);

    [LoggerMessage(LogLevel.Warning, "ignoring {Menu} because of empty path")]
    private static partial void LogIgnoringEmptyPathMenu(this ILogger logger, Type menu);

    public static void LogIgnoringEmptyPathMenu(this ILogger logger, IMenu menu)
        => logger.LogIgnoringEmptyPathMenu(menu.GetType());

    [LoggerMessage(LogLevel.Warning, "ignoring {DuplicateMenu} at {Key}, {CurrentMenu} already present")]
    private static partial void LogIgnoringDuplicateMenu(this ILogger logger, Type duplicateMenu, string key, Type currentMenu);

    public static void LogIgnoringDuplicateMenu(this ILogger logger, string key, IMenu currentMenu, IMenu duplicateMenu)
        => logger.LogIgnoringDuplicateMenu(duplicateMenu.GetType(), key, currentMenu.GetType());

    [LoggerMessage(LogLevel.Warning, "ignoring {Settings} because of empty path")]
    private static partial void LogIgnoringEmptyPathSettings(this ILogger logger, Type settings);

    public static void LogIgnoringEmptyPathSettings(this ILogger logger, ISettings settings)
        => logger.LogIgnoringEmptyPathSettings(settings.GetType());
}
