using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using DefaultEngine.Editor.Api.Plugins;

namespace DefaultEngine.Editor.Internal;

internal sealed class PluginsHelper
{
    private readonly IReadOnlyCollection<Assembly> _pluginCandidates;

    public PluginsHelper(DirectoryInfo directory)
    {
        AssemblyName apiName = typeof(IServicesRegisterer).Assembly.GetName();

        HashSet<string> loadedNames = new(AppDomain.CurrentDomain.GetAssemblies().Select(assembly => assembly.GetName().Name).Where(name => name != null)!, StringComparer.OrdinalIgnoreCase);
        Dictionary<string, bool> checkedNames = new(StringComparer.OrdinalIgnoreCase)
        {
            [string.Empty] = false,
            [apiName.Name!] = true
        };

        List<AssemblyName> pluginCandidates = [];
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
                    pluginCandidates.Add(name);
                    mayBePlugin = true;
                }
            }

            return mayBePlugin;
        }

        HandleNames(directory.EnumerateFiles("*.dll").Select(dll => AssemblyName.GetAssemblyName(dll.FullName)));

        context.Unload();

        _pluginCandidates = [.. pluginCandidates.Select(Assembly.Load)];
    }

    public IEnumerable<TypeInfo> GetPluginsTypes() => _pluginCandidates.AsParallel().SelectMany(assembly => assembly.DefinedTypes);
}
