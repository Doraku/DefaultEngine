using System;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace DefaultEngine.Editor.Internal.Converters;

internal static class StaticConverters
{
    public static FuncValueConverter<Uri, Bitmap?> UriToBitmap { get; } = new FuncValueConverter<Uri, Bitmap?>(uri => uri is null ? null : new Bitmap(AssetLoader.Open(uri)));
}
