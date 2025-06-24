using Avalonia.Controls;
using DefaultApplication.Controls.Metadata;
using DefaultApplication.Internal.Plugins.WorkerServicePlugin.ViewModels;

namespace DefaultApplication.Internal.Plugins.WorkerServicePlugin.Views;

[DataTemplate<OperationViewModel>]
public partial class OperationView : StackPanel
{
    public OperationView()
    {
        InitializeComponent();
    }
}