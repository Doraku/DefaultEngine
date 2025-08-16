using System.Linq;
using Avalonia.Data.Converters;

namespace Avalonia.DefaultLayout.Internal.Converters;

internal static class OperationConverters
{
    public static FuncMultiValueConverter<double, double> SumDoubles { get; } = new(values => values.Sum());

    public static FuncValueConverter<double, double> NegateDouble { get; } = new(value => -value);
}
