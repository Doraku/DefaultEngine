using System;
using System.Collections.Generic;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using DefaultEngine.Editor.Api.Controls.Metadata;
using DefaultEngine.Editor.Api.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultEngine.Editor.Internal.Plugins.DataTemplatesPlugin;

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
                switch (attribute.Lifetime)
                {
                    case ServiceLifetime.Transient:
                        services.TryAddTransient(type);
                        break;

                    case ServiceLifetime.Scoped:
                        services.TryAddScoped(type);
                        break;

                    case ServiceLifetime.Singleton:
                        services.TryAddSingleton(type);
                        break;
                }

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
