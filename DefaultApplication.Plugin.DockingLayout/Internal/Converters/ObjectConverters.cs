using Avalonia.Data.Converters;

namespace DefaultApplication.DockingLayout.Internal.Converters;

internal static class ObjectConverters
{
    public static FuncValueConverter<object, bool> IsClosable { get; } = new FuncValueConverter<object, bool>(content => content?.IsClosable() ?? false);

    public static FuncValueConverter<object, bool> IsHideable { get; } = new FuncValueConverter<object, bool>(content => content?.IsHideable() ?? false);

    public static FuncValueConverter<object, bool> IsMovable { get; } = new FuncValueConverter<object, bool>(content => content?.IsMovable() ?? false);

    public static FuncValueConverter<object, bool> IsFloatable { get; } = new FuncValueConverter<object, bool>(content => content?.IsFloatable() ?? false);
}
