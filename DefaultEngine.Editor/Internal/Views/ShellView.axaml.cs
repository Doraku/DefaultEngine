using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Messaging;
using DefaultEngine.Editor.Api.Mvvm;
using DefaultEngine.Editor.Api.Plugins;
using DefaultEngine.Editor.Internal.ViewModels;
using DefaultUnDo;
using Microsoft.Extensions.Logging;

namespace DefaultEngine.Editor.Internal.Views;

[DataTemplate<ShellViewModel>]
internal sealed partial class ShellView : Border, IRecipient<ShellView.CloseShellViewWindowMessage>
{
    private sealed class Save : IMenuItem
    {
        private readonly ILogger<Save> _logger;
        private readonly IUnDoManager _manager;

        public IReadOnlyList<string> Path { get; } = ["File", "Save"];

        public object? Icon { get; } = new Uri("avares://DefaultEngine.Editor/Resources/Images/DefaultLogo.png");

        public KeyGesture HotKey { get; } = new(Key.S, KeyModifiers.Control);

        public Save(ILogger<Save> logger, IUnDoManager manager)
        {
            _logger = logger;
            _manager = manager;
        }

        public void Execute()
        {
            _manager.Do(
                () => _logger.LogInformation("saved"),
                () => _logger.LogInformation("undone saved"));
        }
    }

    private sealed class Exit : IMenuItem
    {
        private readonly IMessenger _messenger;

        public IReadOnlyList<string> Path { get; } = ["File", "Exit"];

        public KeyGesture HotKey { get; } = new(Key.F4, KeyModifiers.Alt);

        public Exit(IMessenger messenger)
        {
            _messenger = messenger;
        }

        public void Execute() => _messenger.Send<CloseShellViewWindowMessage>();
    }

    internal sealed record CloseShellViewWindowMessage;

    public ShellView()
    {
        InitializeComponent();
    }

    public ShellView(IMessenger messenger)
    {
        InitializeComponent();

        messenger.RegisterAll(this);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (DataContext is not ShellViewModel viewModel)
        {
            return;
        }

        static IEnumerable<ShellViewModel.MenuViewModel> GetAllMenus(ShellViewModel.MenuViewModel menu)
        {
            yield return menu;

            foreach (ShellViewModel.MenuViewModel subMenu in menu.SubMenus?.SelectMany(GetAllMenus) ?? [])
            {
                yield return subMenu;
            }
        }

        Window window = (Window)TopLevel.GetTopLevel(this)!;

        foreach (ShellViewModel.MenuViewModel menu in viewModel.Menus.SelectMany(GetAllMenus).Where(menu => menu.Command != null && menu.HotKey != null))
        {
            window.KeyBindings.Add(new KeyBinding
            {
                Gesture = menu.HotKey!,
                Command = menu.Command!
            });
        }
    }

    public void Receive(CloseShellViewWindowMessage message) => ((Window)TopLevel.GetTopLevel(this)!).Close();
}