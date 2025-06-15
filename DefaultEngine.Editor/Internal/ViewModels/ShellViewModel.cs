using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DefaultEngine.Editor.Api.Plugins;

namespace DefaultEngine.Editor.Internal.ViewModels;

internal sealed partial class ShellViewModel
{
    public sealed class MenuViewModel
    {
        private readonly List<MenuViewModel>? _subMenus;

        public string Name { get; }

        public ICommand? Command { get; }

        public IReadOnlyList<MenuViewModel>? SubMenus => _subMenus;

        public MenuViewModel(string name)
        {
            Name = name;
            _subMenus = [];
        }

        public MenuViewModel(IMenuItem menuItem)
        {
            Name = menuItem.Path[^1];
            Command = new AsyncRelayCommand(menuItem.ExecuteAsync, menuItem.CanExecute, AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
        }

        public void Add(MenuViewModel subMenu)
        {
            _subMenus?.Add(subMenu);
        }
    }

    private readonly List<MenuViewModel> _menus;

    public IReadOnlyCollection<MenuViewModel> Menus => _menus;

    public ShellViewModel(IEnumerable<IMenuItem> menuItems)
    {
        _menus = [];

        foreach (IMenuItem menuItem in menuItems)
        {
            MenuViewModel? currentMenu = null;

            foreach (string part in menuItem.Path.Take(menuItem.Path.Count - 1))
            {
                MenuViewModel? subMenu = (currentMenu?.SubMenus ?? _menus).FirstOrDefault(menu => menu.SubMenus is { } && menu.Name == part);

                if (subMenu is null)
                {
                    subMenu = new MenuViewModel(part);
                    if (currentMenu is null)
                    {
                        _menus.Add(subMenu);
                    }
                    else
                    {
                        currentMenu.Add(subMenu);
                    }
                }

                currentMenu = subMenu;
            }

            if (currentMenu is null)
            {
                _menus.Add(new MenuViewModel(menuItem));
            }
            else
            {
                currentMenu.Add(new MenuViewModel(menuItem));
            }
        }
    }
}
