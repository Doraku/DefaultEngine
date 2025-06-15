using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using DefaultEngine.Editor.Api.Mvvm;
using DefaultEngine.Editor.Api.Plugins;
using DefaultEngine.Editor.Internal.ViewModels;

namespace DefaultEngine.Editor.Internal.Views;

[DataTemplate<ShellViewModel>]
internal sealed partial class ShellView : DockPanel, IRecipient<ShellView.CloseShellViewWindowMessage>
{
    internal sealed record CloseShellViewWindowMessage;

    internal sealed class ExitMenuItem : BaseMenuItem
    {
        private readonly IMessenger _messenger;

        public ExitMenuItem(IMessenger messenger)
            : base("File", "Exit")
        {
            _messenger = messenger;
        }

        protected override void Execute() => _messenger.Send<CloseShellViewWindowMessage>();
    }

    public ShellView(IMessenger messenger)
    {
        InitializeComponent();

        messenger.RegisterAll(this);
    }

    public void Receive(CloseShellViewWindowMessage message) => ((Window)TopLevel.GetTopLevel(this)).Close();
}