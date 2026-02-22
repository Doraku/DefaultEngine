using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Avalonia.Input;

public static class IDataTransferExtensions
{
    private sealed class DataTransfer<T> : IDataTransfer
        where T : class
    {
        public T Value { get; }

        public DataTransfer(T value)
        {
            Value = value;
        }

        IReadOnlyList<DataFormat> IDataTransfer.Formats => [];

        IReadOnlyList<IDataTransferItem> IDataTransfer.Items => [];

        public void Dispose() { }
    }

    extension(IDataTransfer)
    {
        public static IDataTransfer Create<T>(T data) where T : class => new DataTransfer<T>(data);
    }

    public static bool TryGet<T>(this IDataTransfer data, [NotNullWhen(true)] out T? value)
        where T : class
    {
        value = default;

        if (data is DataTransfer<T> wrapper)
        {
            value = wrapper.Value;
        }

        return value != null;
    }
}
