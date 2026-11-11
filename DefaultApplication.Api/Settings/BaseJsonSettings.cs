using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DefaultApplication.Settings;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types")]
public abstract class BaseJsonSettings : ISettings, IAsyncDisposable
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

    protected static async Task<T?> DeserializeAsync<T>(ILogger logger, string jsonPath, CancellationToken cancellationToken)
    {
        if (!File.Exists(jsonPath))
        {
            return default;
        }

        try
        {
            Stream stream = File.OpenRead(jsonPath);

            await using (stream.ConfigureAwait(false))
            {
                return await JsonSerializer.DeserializeAsync<T>(stream, _jsonOptions, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception exception)
        {
            LogDeserializationException(logger, exception);
            return default;
        }
    }

    protected static async Task SerializeAsync<T>(ILogger logger, string jsonPath, T data, CancellationToken cancellationToken)
    {
        string tempPath = System.IO.Path.GetTempFileName();

        try
        {
            Stream stream = File.Create(tempPath);
            await using (stream.ConfigureAwait(false))
            {
                await JsonSerializer.SerializeAsync(stream, data, _jsonOptions, cancellationToken).ConfigureAwait(false);
            }

            File.Move(tempPath, jsonPath, true);
        }
        catch (Exception exception)
        {
            LogSerializationException(logger, exception);
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    public abstract Task ReadAsync(CancellationToken cancellationToken);

    public abstract Task WriteAsync(CancellationToken cancellationToken);

    public async ValueTask DisposeAsync()
    {
        await WriteAsync(CancellationToken.None).ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }
}