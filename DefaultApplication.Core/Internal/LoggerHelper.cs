using System;
using DefaultApplication.Services;
using Microsoft.Extensions.Logging;

namespace DefaultApplication.Internal;

internal static partial class LoggerHelper
{
    [LoggerMessage(LogLevel.Information, "starting with args {Args}")]
    public static partial void LogStart(this ILogger logger, string[] args);

    private static readonly Action<ILogger, Exception?> _logUnhandledExceptionCallback = LoggerMessage.Define(LogLevel.Error, new EventId(0, nameof(LogUnhandledException)), "Unhandled exception");

    public static void LogUnhandledException(this ILogger logger, Exception? exception) => _logUnhandledExceptionCallback(logger, exception);

    private static readonly Action<ILogger, Exception?> _logUnobservedTaskExceptionCallback = LoggerMessage.Define(LogLevel.Error, new EventId(0, nameof(LogUnhandledException)), "Unobserved task exception");

    public static void LogUnobservedTaskException(this ILogger logger, Exception? exception) => _logUnobservedTaskExceptionCallback(logger, exception);

    private static readonly Action<ILogger, Exception?> _logInitializationExceptionCallback = LoggerMessage.Define(LogLevel.Critical, new EventId(0, nameof(LogUnhandledException)), "Error during initialization");

    public static void LogInitializationException(this ILogger logger, Exception? exception) => _logInitializationExceptionCallback(logger, exception);

    private static readonly Action<ILogger, string?, Exception?> _logWorkerServiceExceptionCallback = LoggerMessage.Define<string?>(LogLevel.Error, new EventId(0, nameof(LogUnhandledException)), "Error when running operation {OperationName}");

    public static void LogWorkerServiceException(this ILogger logger, IWorkerService.IOperation operation, Exception? exception) => _logWorkerServiceExceptionCallback(logger, operation.Name, exception);
}
