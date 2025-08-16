using Avalonia.Data.Converters;

namespace Avalonia.DefaultLayout.Internal.Converters;

internal static class ILayoutContentConverters
{
    public static FuncValueConverter<ILayoutContent, bool> IsMoveable { get; } = new FuncValueConverter<ILayoutContent, bool>(content => content?.Options.HasFlag(LayoutOptions.Movable) ?? false);

    public static FuncValueConverter<ILayoutContent, bool> IsStackable { get; } = new FuncValueConverter<ILayoutContent, bool>(content => content?.Options.HasFlag(LayoutOptions.Stackable) ?? false);

    public static FuncValueConverter<ILayoutContent, bool> IsClosable { get; } = new FuncValueConverter<ILayoutContent, bool>(content => content?.Options.HasFlag(LayoutOptions.Closable) ?? false);

    public static FuncValueConverter<ILayoutContent, bool> IsHideable { get; } = new FuncValueConverter<ILayoutContent, bool>(content => content?.Options.HasFlag(LayoutOptions.Hideable) ?? false);

    public static FuncValueConverter<ILayoutContent, bool> IsFloatable { get; } = new FuncValueConverter<ILayoutContent, bool>(content => content?.Options.HasFlag(LayoutOptions.Floatable) ?? false);
}
