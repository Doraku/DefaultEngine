using System;
using System.Collections.Generic;
using DefaultEngine.Editor.Api;
using DefaultEngine.Editor.Api.Plugins;

namespace DefaultEngine.Editor.Internal.Plugins.ShellPlugin.ViewModels;

internal sealed class AboutViewModel
{
    private readonly IEnumerable<IServicesRegisterer> _servicesRegisterers;
    private readonly IEnumerable<IPlugin> _plugins;
    private readonly IEnumerable<IMenu> _menus;

    public Version? Version { get; }

    public AboutViewModel(
        IEnumerable<IServicesRegisterer> servicesRegisterers,
        IEnumerable<IPlugin> plugins,
        IEnumerable<IMenu> menus)
    {
        _servicesRegisterers = servicesRegisterers;
        _plugins = plugins;
        _menus = menus;

        Version = GetType().Assembly.GetName().Version;
    }
}
