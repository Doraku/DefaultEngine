using System.Runtime.CompilerServices;

namespace DefaultApplication.DockingLayout;

internal static class ObjectExtensions
{
    private sealed class LayoutData
    {
        public LayoutOptions Options { get; init; }
    }

    private static readonly ConditionalWeakTable<object, LayoutData> _options = [];

    public static void SetLayoutOptions(this object content, LayoutOptions options)
        => _options.TryAdd(content, new LayoutData { Options = options });

    public static bool IsClosable(this object content) => _options.TryGetValue(content, out LayoutData? data) && data.Options.HasFlag(LayoutOptions.Closable);

    public static bool IsHideable(this object content) => _options.TryGetValue(content, out LayoutData? data) && data.Options.HasFlag(LayoutOptions.Hideable);

    public static bool IsMovable(this object content) => _options.TryGetValue(content, out LayoutData? data) && data.Options.HasFlag(LayoutOptions.Movable);

    public static bool IsFloatable(this object content) => _options.TryGetValue(content, out LayoutData? data) && data.Options.HasFlag(LayoutOptions.Floatable);
}
