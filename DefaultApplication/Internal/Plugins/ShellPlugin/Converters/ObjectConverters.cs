using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace DefaultApplication.Internal.Plugins.ShellPlugin.Converters;

internal static class ObjectConverters
{
    public static FuncValueConverter<object, object?> ToKnownIconType { get; } = new FuncValueConverter<object, object?>(
        value => value switch
        {
            StaticResourceExtension resource when resource.ResourceKey is { } => Application.Current?.TryFindResource(resource.ResourceKey, out object? resourceValue) ?? false ? resourceValue : null,
            DynamicResourceExtension resource when resource.ResourceKey is { } => Application.Current?.TryFindResource(resource.ResourceKey, out object? resourceValue) ?? false ? resourceValue : null,
            Uri uri => new Bitmap(AssetLoader.Open(uri)),
            _ => value
        });
}
