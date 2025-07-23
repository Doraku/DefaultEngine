using System.Collections.Generic;
using System.Linq;
using Avalonia.Data.Converters;

namespace DefaultApplication.DockingLayout.Internal.Converters;

internal static class OperationConverters
{
    public static FuncMultiValueConverter<double, double> SumDoubles { get; } = new(values => values.Sum());

    public static FuncValueConverter<double, double> NegateDouble { get; } = new(value => -value);

    public static IMultiValueConverter SubstractDoubles { get; } = new FuncMultiValueConverter<double, double>(
        values => values is List<double> doubles && doubles.Count is 2 ? doubles[0] - doubles[1] : default);
}
