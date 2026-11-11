using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
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

    private readonly ISettings[] _settings;

    public Plugin(IEnumerable<ISettings> settings, Application? application = null)
    {
        _settings = [.. settings];

        if (application is null)
        {
            return;
        }

        Uri baseUri = new("avares://DefaultApplication.Core");
        Uri resourcesUri = new(baseUri, "Internal/Plugins/SettingsPlugin/Resources/");

        application.Dispatcher.Invoke(() => application.Resources.MergedDictionaries.Add(new ResourceInclude(baseUri) { Source = new Uri(resourcesUri, "Resources.axaml") }));

        application.DataTemplates.AddRange(settings.Select(part => new SettingsTemplate(part.GetType())));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (ISettings setting in _settings)
        {
            await setting.ReadAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}