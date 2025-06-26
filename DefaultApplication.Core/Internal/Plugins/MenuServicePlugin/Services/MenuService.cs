using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Input;
using DefaultApplication.DependencyInjection;
using DefaultApplication.Services;
using Microsoft.Extensions.Logging;

namespace DefaultApplication.Internal.Plugins.MenuServicePlugin.Services;

internal sealed class MenuService : IMenuService
{
    private sealed class MenuItem : IMenuService.IMenuCommand
    {
        private readonly List<MenuItem>? _subMenus;
        private readonly Func<bool>? _canExecute;
        private readonly Func<Task>? _executeAsync;

        private Task? _executionTask;

        public int Order { get; }

        public string Header { get; }

        public KeyGesture? HotKey { get; }

        public object? Icon { get; }

        public IReadOnlyCollection<IMenuService.IMenuCommand>? SubCommands => _subMenus;

        public MenuItem(string name)
        {
            Header = name;
            _subMenus = [];
        }

        public MenuItem(IMenu menu)
        {
            Order = menu.Order;
            Header = menu.Path[^1];
            Icon = menu.Icon;

            if (menu is IBaseCommandMenu commandMenu)
            {
                (_executeAsync, _canExecute) = menu switch
                {
                    ICommandMenu sync => new(Wrap(sync.Execute), commandMenu.CanExecute),
                    IAsyncCommandMenu async => (async.ExecuteAsync, commandMenu.CanExecute),
                    _ => (default(Func<Task>), default(Func<bool>))
                };
                HotKey = _executeAsync != null ? commandMenu.HotKey : null;
            }
            else
            {
                _subMenus = [];
            }

            _executionTask = Task.CompletedTask;
        }

        private static Func<Task> Wrap(Action execute)
        {
            return () =>
            {
                execute();
                return Task.CompletedTask;
            };
        }

        public static int Compare(MenuItem menu1, MenuItem menu2)
        {
            int comparison = menu1.Order.CompareTo(menu2.Order);
            comparison = comparison != 0 ? comparison : string.Compare(menu1.Header, menu2.Header, StringComparison.OrdinalIgnoreCase);
            comparison = comparison != 0 ? comparison : (menu1.SubCommands?.Count ?? int.MaxValue).CompareTo(menu2.SubCommands?.Count ?? int.MaxValue);

            return comparison;
        }

        public void Add(MenuItem subMenu)
        {
            _subMenus?.Add(subMenu);
            _subMenus?.Sort(Compare);
        }

        public bool CanExecute() => _executeAsync is { } && _executionTask is { IsCompleted: true } && (_canExecute?.Invoke() ?? true);

        public async Task ExecuteAsync()
        {
            _executionTask = _executeAsync?.Invoke() ?? Task.CompletedTask;

            CanExecuteChanged?.Invoke(this, EventArgs.Empty);

            await _executionTask.ConfigureAwait(true);

            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #region ICommand

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => CanExecute();

        public void Execute(object? parameter) => _ = ExecuteAsync();

        #endregion
    }

    private sealed class HotKeyCommand : ICommand
    {
        private readonly ICommand _command;
        private readonly Func<bool> _canExecute;

        public HotKeyCommand(ICommand command, Func<bool> canExecute)
        {
            _command = command;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => _command.CanExecuteChanged += value;
            remove => _command.CanExecuteChanged -= value;
        }

        public bool CanExecute(object? parameter) => _canExecute() && _command.CanExecute(parameter);

        public void Execute(object? parameter) => _command.Execute(parameter);
    }

    private readonly List<MenuItem> _menus;

    public bool IsEnabled { get; set; }

    public IReadOnlyCollection<IMenuService.IMenuCommand> Commands => _menus;

    public MenuService(ILogger<MenuService> logger, IEnumerable<IMenu> menus, IDelayed<TopLevel> topLevel)
    {
        _menus = [];

        IsEnabled = true;

        Dictionary<string, IMenu> distinctMenus = [];

        static string GetKey(IEnumerable<string> path) => string.Join('>', path ?? []);

        foreach (IMenu menu in menus)
        {
            string key = GetKey(menu.Path);

            if (string.IsNullOrEmpty(key))
            {
                logger.LogIgnoringEmptyPathMenu(menu);
                continue;
            }

            if (menu is not IBaseCommandMenu)
            {
                key += '>';
            }

            if (distinctMenus.TryGetValue(key, out IMenu? firstMenu))
            {
                logger.LogIgnoringDuplicateMenu(key, firstMenu, menu);
                continue;
            }

            distinctMenus.Add(key, menu);
        }

        Dictionary<string, MenuItem> viewModels = [];

        while (distinctMenus.Count > 0)
        {
            string key = distinctMenus.Keys.First();
            if (distinctMenus.Remove(key, out IMenu? value))
            {
                MenuItem viewModel = new(value);
                viewModels.Add(key, viewModel);
                bool isNew = true;

                for (int i = value.Path.Count - 2; i >= 0; --i)
                {
                    string parentKey = GetKey(value.Path.Take(i + 1)) + '>';
                    isNew = !viewModels.TryGetValue(parentKey, out MenuItem? parentViewModel);

                    if (isNew)
                    {
                        parentViewModel = distinctMenus.Remove(parentKey, out IMenu? parentMenu) ? new MenuItem(parentMenu) : new MenuItem(value.Path[i]);
                        viewModels.Add(parentKey, parentViewModel);
                    }

                    parentViewModel!.Add(viewModel);
                    viewModel = parentViewModel;
                }

                if (isNew)
                {
                    _menus.Add(viewModel);
                }
            }
        }

        _menus.Sort(MenuItem.Compare);

        topLevel.Task.ContinueWith(
            async task => RegisterMenusHotKey(await task.ConfigureAwait(true)),
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);
    }

    private void RegisterMenusHotKey(TopLevel topLevel)
    {
        static IEnumerable<IMenuService.IMenuCommand> GetAllMenus(IMenuService.IMenuCommand menu)
        {
            yield return menu;

            foreach (IMenuService.IMenuCommand subMenu in menu.SubCommands?.SelectMany(GetAllMenus) ?? [])
            {
                yield return subMenu;
            }
        }

        foreach (IMenuService.IMenuCommand menu in _menus.SelectMany(GetAllMenus).Where(menu => menu.HotKey is { }))
        {
            topLevel.KeyBindings.Add(new KeyBinding
            {
                Gesture = menu.HotKey!,
                Command = new HotKeyCommand(menu, () => IsEnabled)
            });
        }
    }
}
