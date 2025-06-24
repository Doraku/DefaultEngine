using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DefaultApplication.Plugins;

namespace DefaultApplication.Internal.Plugins.AboutPlugin.ViewModels;

internal sealed class AboutViewModel
{
    public sealed record PluginInfo(
        string? Name,
        Version? Version)
    {
        public PluginInfo(AssemblyName assemblyName)
            : this(assemblyName.Name, assemblyName.Version)
        { }

        public static PluginInfo From(Type type) => new(type.Assembly.GetName());
    }

    public Version? MainVersion { get; }

    public IEnumerable<IGrouping<PluginInfo, Type>> Items { get; }

    public AboutViewModel(
        IEnumerable<IServicesRegisterer> servicesRegisterers,
        IEnumerable<IPlugin> plugins,
        IEnumerable<IMenu> menus)
    {
        MainVersion = (Assembly.GetEntryAssembly() ?? GetType().Assembly).GetName().Version;

        Items = [.. Array.Empty<object>()
            .Concat(servicesRegisterers)
            .Concat(plugins)
            .Concat(menus)
            .Distinct()
            .Select(item => item.GetType())
            .OrderBy(type => type.FullName)
            .GroupBy(PluginInfo.From)
            .OrderBy(group => group.Key.Name)
            .ThenBy(group => group.Key.Version)];
    }
}
