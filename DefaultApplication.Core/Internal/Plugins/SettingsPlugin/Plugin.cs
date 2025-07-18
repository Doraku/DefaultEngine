using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Threading;
using DefaultApplication.Internal.Plugins.SettingsPlugin.Controls.Templates;
using DefaultApplication.Internal.Plugins.SettingsPlugin.ViewModels;
using DefaultApplication.Plugins;
using DefaultApplication.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultApplication.Internal.Plugins.SettingsPlugin;

internal sealed class Plugin : IPlugin
{
    private sealed class Registerer : IServiceRegisterer
    {
        private readonly PluginsHelper _plugins;

        public Registerer(PluginsHelper pluginsHelper)
        {
            _plugins = pluginsHelper;
        }

        public void Register(IServiceCollection services)
        {
            services.TryAddTransient<SettingsViewModel>();

            foreach (Type type in _plugins.GetTypes().GetInstanciableImplementation<ISettings>())
            {
                services.AddAsSingletonImplementation<ISettings>(type);
            }
        }
    }

    public Plugin(IEnumerable<ISettings> settings, Application? application = null)
    {
        foreach (ISettings setting in settings)
        {
            setting.Read();
        }

        if (application is null)
        {
            return;
        }

        Uri baseUri = new("avares://DefaultApplication.Core");
        Uri resourcesUri = new(baseUri, "Internal/Plugins/SettingsPlugin/Resources/");

        Dispatcher.UIThread.Invoke(() => application.Resources.MergedDictionaries.Add(new ResourceInclude(baseUri) { Source = new Uri(resourcesUri, "Resources.axaml") }));

        application.DataTemplates.AddRange(settings.Select(part => new SettingsTemplate(part.GetType())));
    }
}