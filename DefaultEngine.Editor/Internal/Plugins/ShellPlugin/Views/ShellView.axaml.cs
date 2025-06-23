using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Messaging;
using DefaultEngine.Editor.Api.Controls.Metadata;
using DefaultEngine.Editor.Api.DependencyInjection;
using DefaultEngine.Editor.Api.Services;
using DefaultEngine.Editor.Internal.Plugins.ShellPlugin.Menus;
using DefaultEngine.Editor.Internal.Plugins.ShellPlugin.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DefaultEngine.Editor.Internal.Plugins.ShellPlugin.Views;

[DataTemplate<ShellViewModel>(ServiceLifetime.Singleton)]
internal sealed partial class ShellView : Border, IRecipient<ExitMenu.Message>, IContentDialogService
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

    private readonly IDelayedItem<Window> _mainWindow;

    public ShellView(IDelayedItem<Window> mainWindow, IMessenger messenger)
    {
        InitializeComponent();

        messenger.RegisterAll(this);
        _mainWindow = mainWindow;
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

        TopLevel topLevel = await _mainWindow.Value.ConfigureAwait(true);

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

    public async void Receive(ExitMenu.Message message) => (await _mainWindow.Value.ConfigureAwait(true)).Close();

    #endregion

    #region IContentDialogService

    public async Task<IContentDialogService.DialogResult> ShowAsync(object content, CancellationToken cancellationToken)
        => await ContentDialogHost.ShowAsync(
            await _mainWindow.Value.ConfigureAwait(false),
            content,
            cancellationToken).ConfigureAwait(false);

    #endregion
}