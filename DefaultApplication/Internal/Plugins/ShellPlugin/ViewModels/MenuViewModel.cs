using System;
using System.Collections.Generic;
using System.Windows.Input;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;
using DefaultApplication;

namespace DefaultApplication.Internal.Plugins.ShellPlugin.ViewModels;

internal sealed class MenuViewModel
{
    private readonly List<MenuViewModel>? _subMenus;

    public int Order { get; }

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

    public MenuViewModel(IMenu menu)
    {
        Order = menu.Order;
        Header = menu.Path[^1];
        Icon = menu.Icon;

        if (menu is IBaseCommandMenu commandMenu)
        {
            HotKey = commandMenu.HotKey;
            Command = menu switch
            {
                ICommandMenu sync => new RelayCommand(sync.Execute, commandMenu.CanExecute),
                IAsyncCommandMenu async => new AsyncRelayCommand(async.ExecuteAsync, commandMenu.CanExecute, AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler),
                _ => null
            };
        }
        else
        {
            _subMenus = [];
        }
    }

    public static int Compare(MenuViewModel menu1, MenuViewModel menu2)
    {
        int comparison = menu1.Order.CompareTo(menu2.Order);
        comparison = comparison != 0 ? comparison : string.Compare(menu1.Header, menu2.Header, StringComparison.OrdinalIgnoreCase);
        comparison = comparison != 0 ? comparison : (menu1.SubMenus?.Count ?? int.MaxValue).CompareTo(menu2.SubMenus?.Count ?? int.MaxValue);

        return comparison;
    }

    public void Add(MenuViewModel subMenu)
    {
        _subMenus?.Add(subMenu);
        _subMenus?.Sort(Compare);
    }
}
