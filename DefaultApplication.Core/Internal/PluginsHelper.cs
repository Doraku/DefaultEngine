using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using DefaultApplication.Plugins;

namespace DefaultApplication.Internal;

internal sealed class PluginsHelper
{
    private readonly IReadOnlyCollection<Assembly> _candidates;

    public PluginsHelper()
    {
        AssemblyName apiName = typeof(IServiceRegisterer).Assembly.GetName();

        HashSet<string> loadedNames = new(AppDomain.CurrentDomain.GetAssemblies().Select(assembly => assembly.GetName().Name).Where(name => name != null)!, StringComparer.OrdinalIgnoreCase);
        Dictionary<string, bool> checkedNames = new(StringComparer.OrdinalIgnoreCase)
        {
            [string.Empty] = false,
            [apiName.Name!] = true
        };

        List<AssemblyName> candidates = [];
        AssemblyLoadContext context = new("Plugins", true);

        bool HandleNames(IEnumerable<AssemblyName> names)
        {
            bool mayBePlugin = false;

            foreach (AssemblyName name in names)
            {
                if (checkedNames.TryGetValue(name.Name ?? string.Empty, out bool couldBePlugin))
                {
                    mayBePlugin |= couldBePlugin;
                    continue;
                }

                bool isLoaded = loadedNames.Contains(name.Name!);

                Assembly assembly = isLoaded ? Assembly.Load(name) : context.LoadFromAssemblyName(name);

                couldBePlugin = HandleNames(assembly.GetReferencedAssemblies());
                checkedNames[name.Name!] = couldBePlugin;
                if (couldBePlugin)
                {
                    candidates.Add(name);
                    mayBePlugin = true;
                }
            }

            return mayBePlugin;
        }

        HandleNames([Assembly.GetEntryAssembly()!.GetName()]);

        context.Unload();

        _candidates = [.. candidates.Select(Assembly.Load)];
    }

    public IEnumerable<TypeInfo> GetTypes() => _candidates.AsParallel().SelectMany(assembly => assembly.DefinedTypes);
}
