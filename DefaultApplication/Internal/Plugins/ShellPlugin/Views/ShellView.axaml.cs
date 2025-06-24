using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Messaging;
using DefaultApplication.Controls.Metadata;
using DefaultApplication.DependencyInjection;
using DefaultApplication.Internal.Plugins.ShellPlugin.Menus;
using DefaultApplication.Internal.Plugins.ShellPlugin.ViewModels;

namespace DefaultApplication.Internal.Plugins.ShellPlugin.Views;

[DataTemplate<ShellViewModel>]
internal sealed partial class ShellView : Border, IRecipient<ExitMenu.Message>
{
    private sealed class HotKeyCommand : ICommand
    {
        private readonly ICommand _command;
        private readonly InputElement _parent;

        public HotKeyCommand(ICommand command, InputElement parent)
        {
            _command = command;
            _parent = parent;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => _command.CanExecuteChanged += value;
            remove => _command.CanExecuteChanged -= value;
        }

        public bool CanExecute(object? parameter) => _parent.IsEnabled && _command.CanExecute(parameter);

        public void Execute(object? parameter) => _command.Execute(parameter);
    }

    private readonly IDelayed<TopLevel> _mainTopLevel;

    public ShellView(IDelayed<TopLevel> mainTopLevel, IMessenger messenger)
    {
        InitializeComponent();

        messenger.RegisterAll(this);
        _mainTopLevel = mainTopLevel;
    }

    private async void RegisterMenusHotKey(IEnumerable<MenuViewModel> menus)
    {
        static IEnumerable<MenuViewModel> GetAllMenus(MenuViewModel menu)
        {
            yield return menu;

            foreach (MenuViewModel subMenu in menu.SubMenus?.SelectMany(GetAllMenus) ?? [])
            {
                yield return subMenu;
            }
        }

        TopLevel topLevel = await _mainTopLevel.ConfigureAwait(true);

        foreach (MenuViewModel menu in menus.SelectMany(GetAllMenus).Where(menu => menu.Command is { } && menu.HotKey is { }))
        {
            topLevel.KeyBindings.Add(new KeyBinding
            {
                Gesture = menu.HotKey!,
                Command = new HotKeyCommand(menu.Command!, TopMenu)
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

    #region IRecipient

    public async void Receive(ExitMenu.Message message) => ((Window)await _mainTopLevel.Task.ConfigureAwait(true)).Close();

    #endregion
}