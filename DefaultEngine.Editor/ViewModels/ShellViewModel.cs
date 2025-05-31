using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using DefaultEngine.Editor.Services;

namespace DefaultEngine.Editor.ViewModels;

internal sealed partial class ShellViewModel
{
    private readonly PluginsService _plugins;

    public ShellViewModel(PluginsService plugins)
    {
        _plugins = plugins;
    }

    [RelayCommand]
    private Task OnReloadPlugins()
    {
        return _plugins.Load();
    }
}
