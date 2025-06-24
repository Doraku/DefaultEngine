using System;
using System.Collections.Generic;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using DefaultApplication.Controls.Metadata;
using DefaultApplication.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultApplication.Internal.Plugins;

internal sealed class DataTemplatesPlugin : IPlugin
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

    public DataTemplatesPlugin(IServiceProvider provider, Application application, DataTemplates dataTemplates)
    {
        foreach ((Type type, DataTemplateAttribute attribute) in dataTemplates)
        {
            application.DataTemplates.Add(new FuncDataTemplate(attribute.DataType, (_, _) => provider.GetRequiredService(type) as Control, true));
        }

        dataTemplates.Clear();
    }
}
