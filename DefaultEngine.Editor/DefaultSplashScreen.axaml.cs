using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;

namespace DefaultEngine.Editor;

internal sealed partial class DefaultSplashScreen : Window
{
    public DefaultSplashScreen()
    {
        InitializeComponent();
    }

    private void OnClicked(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Visual visual
            && e.GetCurrentPoint(visual).Properties.IsLeftButtonPressed)
        {
            Hide();
        }
    }

    public async Task SetInformations(string value)
    {
        await Dispatcher.UIThread.InvokeAsync(() => InformationsTextBlock.Text = value);
    }
}