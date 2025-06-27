using Avalonia.Controls;
using DefaultApplication.Browser.ViewModels;
using DefaultApplication.Controls.Metadata;

namespace DefaultApplication.Browser.Views;

[DataTemplate<MainViewModel>]
public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }
}
