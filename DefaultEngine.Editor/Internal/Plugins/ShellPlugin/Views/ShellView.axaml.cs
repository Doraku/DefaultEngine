using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DefaultEngine.Editor.Api.Controls.Behaviors;
using DefaultEngine.Editor.Api.Controls.Metadata;
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

        TopLevel? topLevel = TopLevel.GetTopLevel(this);

        if (topLevel is { })
        {
            foreach (MenuViewModel menu in menus.SelectMany(GetAllMenus).Where(menu => menu.Command is { } && menu.HotKey is { }))
            {
                topLevel.KeyBindings.Add(new KeyBinding
                {
                    Gesture = menu.HotKey!,
                    Command = new HotKeyCommand(menu.Command!, TopMenu)
                });
            }
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

    public void Receive(ExitMenu.Message message) => ((Window)TopLevel.GetTopLevel(this)!).Close();

    #endregion

    #region IContentDialogService

    private TaskCompletionSource<IContentDialogService.DialogResult>? _contentDialogResult;

    [RelayCommand]
    public void OnContentDialogReturn(IContentDialogService.DialogResult result) => _contentDialogResult?.TrySetResult(result);

    private void OnContentDialogKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key is Key.Escape
            && ContentDialog.GetNoneContent(ContentDialogPresenter.Child) is { })
        {
            _contentDialogResult?.TrySetResult(IContentDialogService.DialogResult.None);
        }
        else if (e.Key is Key.Enter
            && ContentDialog.GetPrimaryContent(ContentDialogPresenter.Child) is { }
            && ContentDialog.GetCanReturnPrimary(ContentDialogPresenter.Child))
        {
            _contentDialogResult?.TrySetResult(IContentDialogService.DialogResult.Primary);
        }
    }

    private void OnContentDialogPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == IsVisibleProperty
            && !e.GetNewValue<bool>())
        {
            ContentDialogHost.Tag = null;
        }
    }

    public async Task<IContentDialogService.DialogResult> ShowAsync(object content, CancellationToken cancellationToken)
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            return await Dispatcher.UIThread.InvokeAsync(() => ShowAsync(content, cancellationToken)).ConfigureAwait(false);
        }

        TopLevel? topLevel = TopLevel.GetTopLevel(this);

        if (topLevel is null)
        {
            return IContentDialogService.DialogResult.None;
        }

        _contentDialogResult = new TaskCompletionSource<IContentDialogService.DialogResult>();

        using (cancellationToken.Register(() => _contentDialogResult.TrySetResult(IContentDialogService.DialogResult.None)))
        {
            topLevel.KeyDown += OnContentDialogKeyDown;
            ContentDialogHost.Tag = content;
            ContentDialogHost.Opacity = 1;

            IContentDialogService.DialogResult result;

            try
            {
                result = await _contentDialogResult.Task.ConfigureAwait(true);
            }
            finally
            {
                ContentDialogHost.Opacity = 0;
                topLevel.KeyDown -= OnContentDialogKeyDown;
            }

            return result;
        }
    }

    #endregion
}