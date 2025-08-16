using System;

namespace Avalonia.DefaultLayout;

[Flags]
public enum LayoutOptions
{
    None = 0,
    Closable = 1 << 0,
    Hideable = 1 << 1,
    Movable = 1 << 2,
    Stackable = 1 << 3,
    Floatable = 1 << 4
}