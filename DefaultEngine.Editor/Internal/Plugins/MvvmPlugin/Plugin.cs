using System;
using System.Collections.Generic;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using DefaultEngine.Editor.Api.Mvvm;
using DefaultEngine.Editor.Api.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultEngine.Editor.Internal.Plugins.MvvmPlugin;

internal sealed class Plugin : IPlugin
{
    internal sealed class DataTemplates : List<(Type, DataTemplateAttribute)>;

    internal sealed class Registerer : IServicesRegisterer
    {
        private readonly PluginsHelper _plugins;

        public Registerer(PluginsHelper plugins)
        {
            _plugins = plugins;
        }

        public void Register(IServiceCollection services)
        {
            DataTemplates dataTemplates = [];

            foreach ((Type type, DataTemplateAttribute attribute) in _plugins.GetPluginsTypes().GetInstanciableImplementation<Control>().GetTypesWithAttribute<DataTemplateAttribute>())
            {
                services.TryAddTransient(type);
                dataTemplates.Add((type, attribute));
            }

            services.AddSingleton(dataTemplates);
        }
    }

    public Plugin(IServiceProvider provider, Application application, DataTemplates dataTemplates)
    {
        foreach ((Type type, DataTemplateAttribute attribute) in dataTemplates)
        {
            application.DataTemplates.Add(new FuncDataTemplate(attribute.DataType, (_, _) => provider.GetRequiredService(type) as Control, true));
        }

        dataTemplates.Clear();
    }
}
