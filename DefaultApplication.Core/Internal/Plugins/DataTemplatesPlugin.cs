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

    internal sealed class Registerer : IServiceRegisterer
    {
        private readonly PluginsHelper _plugins;
        private readonly Application? _application;

        public Registerer(PluginsHelper plugins, Application? application = null)
        {
            _plugins = plugins;
            _application = application;
        }

        public void Register(IServiceCollection services)
        {
            if (_application is null)
            {
                return;
            }

            DataTemplates dataTemplates = [];

            foreach ((Type type, DataTemplateAttribute attribute) in _plugins.GetTypes().GetInstanciableImplementation<Control>().GetTypesWithAttribute<DataTemplateAttribute>())
            {
                services.TryAddTransient(type);

                dataTemplates.Add((type, attribute));
            }

            services.AddSingleton(dataTemplates);
        }
    }

    public DataTemplatesPlugin(IServiceProvider provider, Application? application = null, DataTemplates? dataTemplates = null)
    {
        if (application is null)
        {
            return;
        }

        foreach ((Type type, DataTemplateAttribute attribute) in dataTemplates!)
        {
            application.DataTemplates.Add(new FuncDataTemplate(attribute.DataType, (_, _) => provider.GetRequiredService(type) as Control, true));
        }

        dataTemplates.Clear();
    }
}
