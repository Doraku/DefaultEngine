using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;
using DefaultEngine.Editor.Api.Plugins;

namespace DefaultEngine.Editor.Internal.ViewModels;

internal sealed class ShellViewModel
{
    public sealed class MenuViewModel
    {
        private readonly List<MenuViewModel>? _subMenus;

        public string Header { get; }

        public ICommand? Command { get; }

        public KeyGesture? HotKey { get; }

        public object? Icon { get; }

        public IReadOnlyList<MenuViewModel>? SubMenus => _subMenus;

        public MenuViewModel(string name)
        {
            Header = name;
            _subMenus = [];
        }

        public MenuViewModel(ICommonMenuItem menuItem)
        {
            Header = menuItem.Path[^1];
            Command = menuItem switch
            {
                IMenuItem sync => new RelayCommand(sync.Execute, menuItem.CanExecute),
                IAsyncMenuItem async => new AsyncRelayCommand(async.ExecuteAsync, menuItem.CanExecute, AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler),
                ICancellableAsyncMenuItem cancellableAsync => new AsyncRelayCommand(cancellableAsync.ExecuteAsync, menuItem.CanExecute, AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler),
                _ => null
            };
            HotKey = menuItem.HotKey;
            Icon = menuItem.Icon;
        }

        public void Add(MenuViewModel subMenu)
        {
            _subMenus?.Add(subMenu);
        }
    }

    private readonly List<MenuViewModel> _menus;

    public IReadOnlyCollection<MenuViewModel> Menus => _menus;

    public ShellViewModel(IEnumerable<ICommonMenuItem> menuItems)
    {
        _menus = [];

        foreach (ICommonMenuItem menuItem in menuItems)
        {
            MenuViewModel? currentMenu = null;

            foreach (string part in menuItem.Path.Take(menuItem.Path.Count - 1))
            {
                MenuViewModel? subMenu = (currentMenu?.SubMenus ?? _menus).FirstOrDefault(menu => menu.SubMenus is { } && menu.Header == part);

                if (subMenu is null)
                {
                    subMenu = new MenuViewModel(part);

                    (currentMenu is null ? (Action<MenuViewModel>)_menus.Add : currentMenu.Add)(subMenu);
                }

                currentMenu = subMenu;
            }

            (currentMenu is null ? (Action<MenuViewModel>)_menus.Add : currentMenu.Add)(new MenuViewModel(menuItem));
        }
    }
}
