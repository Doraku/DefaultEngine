using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace DefaultApplication.Settings;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1063:Implement IDisposable Correctly")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types")]
public abstract class BaseJsonSettings : ISettings, IDisposable
{
    private static readonly JsonSerializerOptions _jsonOptions;

    private static readonly Action<ILogger, Exception?> _logDeserializationException = LoggerMessage.Define(LogLevel.Critical, new EventId(0, nameof(LogDeserializationException)), "settings deserialization exception");
    private static readonly Action<ILogger, Exception?> _logSerializationException = LoggerMessage.Define(LogLevel.Critical, new EventId(0, nameof(LogSerializationException)), "settings serialization exception");

    protected ILogger Logger { get; }

    public IReadOnlyList<string> Path { get; }

    static BaseJsonSettings()
    {
        _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.General)
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
        };
        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }

    protected BaseJsonSettings(ILogger logger, params ReadOnlySpan<string> path)
    {
        Logger = logger;
        Path = [.. path];
    }

    private static void LogDeserializationException(ILogger logger, Exception? exception) => _logDeserializationException(logger, exception);

    private static void LogSerializationException(ILogger logger, Exception? exception) => _logSerializationException(logger, exception);

    protected static T? Deserialize<T>(ILogger logger, string jsonPath)
    {
        if (!File.Exists(jsonPath))
        {
            return default;
        }

        try
        {
            using Stream stream = File.OpenRead(jsonPath);
            return JsonSerializer.Deserialize<T>(stream, _jsonOptions);
        }
        catch (Exception exception)
        {
            LogDeserializationException(logger, exception);
            return default;
        }
    }

    protected static void Serialize<T>(ILogger logger, string jsonPath, T data)
    {
        try
        {
            using Stream stream = File.Open(jsonPath, FileMode.OpenOrCreate);

            JsonSerializer.Serialize(stream, data, _jsonOptions);

            stream.SetLength(stream.Position);
        }
        catch (Exception exception)
        {
            LogSerializationException(logger, exception);
        }
    }

    public abstract void Read();

    public abstract void Write();

    public void Dispose()
    {
        Write();
        GC.SuppressFinalize(this);
    }
}