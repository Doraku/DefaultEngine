using System;
using Avalonia.Controls;
using DefaultApplication.Controls.Metadata;
using DefaultApplication.Internal.Plugins.MenuServicePlugin.Services;

namespace DefaultApplication;

[DataTemplate<MenuService>]
public partial class MenuView : Menu
{
    public MenuView()
    {
        InitializeComponent();
    }

    protected override Type StyleKeyOverride => typeof(Menu);
}