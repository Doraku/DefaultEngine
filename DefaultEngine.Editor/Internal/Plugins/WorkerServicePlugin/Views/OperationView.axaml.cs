using Avalonia.Controls;
using DefaultEngine.Editor.Api.Controls.Metadata;
using DefaultEngine.Editor.Internal.Plugins.WorkerServicePlugin.ViewModels;

namespace DefaultEngine.Editor.Internal.Plugins.WorkerServicePlugin.Views;

[DataTemplate<OperationViewModel>]
public partial class OperationView : StackPanel
{
    public OperationView()
    {
        InitializeComponent();
    }
}