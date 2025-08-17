using System;
using Avalonia.Controls;

namespace Avalonia.DefaultLayout.Internal.Views;

public sealed partial class StackedLayoutContentView : TabControl
{
    protected override Type StyleKeyOverride => typeof(TabControl);

    public StackedLayoutContentView()
    {
        InitializeComponent();
    }
}