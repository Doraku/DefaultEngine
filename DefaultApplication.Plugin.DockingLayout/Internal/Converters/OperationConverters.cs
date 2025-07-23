using System.Collections.Generic;
using Avalonia.Data.Converters;

namespace DefaultApplication.DockingLayout.Internal.Converters;

internal static class OperationConverters
{
    public static IMultiValueConverter SubstractDoubles { get; } = new FuncMultiValueConverter<double, double>(
        values => values is List<double> doubles && doubles.Count is 2 ? doubles[0] - doubles[1] : default);
}
