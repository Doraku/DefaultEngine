using DefaultApplication.Services;

namespace DefaultApplication.Internal.Plugins.ShellPlugin.ViewModels;

internal sealed class ShellViewModel
{
    public IMenuService Menus { get; }

    public ShellViewModel(IMenuService menus)
    {
        Menus = menus;
    }
}
