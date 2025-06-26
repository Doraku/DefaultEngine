using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using DefaultApplication.Controls.Metadata;
using DefaultApplication.DependencyInjection;
using DefaultApplication.Internal.Plugins.ShellPlugin.Menus;
using DefaultApplication.Internal.Plugins.ShellPlugin.ViewModels;

namespace DefaultApplication.Internal.Plugins.ShellPlugin.Views;

[DataTemplate<ShellViewModel>]
internal sealed partial class ShellView : Border, IRecipient<ExitMenu.Message>
{
    private readonly IDelayed<TopLevel> _mainTopLevel;

    public ShellView(IDelayed<TopLevel> mainTopLevel, IMessenger messenger)
    {
        InitializeComponent();

        messenger.RegisterAll(this);
        _mainTopLevel = mainTopLevel;
    }

    #region IRecipient

    public async void Receive(ExitMenu.Message message) => ((Window)await _mainTopLevel.Task.ConfigureAwait(true)).Close();

    #endregion
}