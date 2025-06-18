using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Messaging;
using DefaultEngine.Editor.Api.Mvvm;
using DefaultEngine.Editor.Internal.Plugins.ShellPlugin.Menus;
using DefaultEngine.Editor.Internal.Plugins.ShellPlugin.ViewModels;

namespace DefaultEngine.Editor.Internal.Plugins.ShellPlugin.Views;

[DataTemplate<ShellViewModel>]
internal sealed partial class ShellView : Border, IRecipient<ExitMenu.Message>
{
    public ShellView()
    {
        InitializeComponent();
    }

    public ShellView(IMessenger messenger)
    {
        InitializeComponent();

        messenger.RegisterAll(this);
    }

    private void RegisterMenusHotKey(IEnumerable<MenuViewModel> menus)
    {
        static IEnumerable<MenuViewModel> GetAllMenus(MenuViewModel menu)
        {
            yield return menu;

            foreach (MenuViewModel subMenu in menu.SubMenus?.SelectMany(GetAllMenus) ?? [])
            {
                yield return subMenu;
            }
        }

        Window window = (Window)TopLevel.GetTopLevel(this)!;

        foreach (MenuViewModel menu in menus.SelectMany(GetAllMenus).Where(menu => menu.Command is { } && menu.HotKey is { }))
        {
            window.KeyBindings.Add(new KeyBinding
            {
                Gesture = menu.HotKey!,
                Command = menu.Command!
            });
        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (DataContext is not ShellViewModel viewModel)
        {
            return;
        }

        RegisterMenusHotKey(viewModel.Menus);
    }

    public void Receive(ExitMenu.Message message) => ((Window)TopLevel.GetTopLevel(this)!).Close();
}