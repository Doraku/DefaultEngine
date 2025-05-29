using System;
using System.IO;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis.MSBuild;

namespace DefaultEngine.Editor.Services;

internal sealed class PluginsService : IDisposable
{
    private readonly AssemblyLoadContext _context;
    private readonly string _buildPath;

    public PluginsService()
    {
        _context = new AssemblyLoadContext("Plugins", true);
        _buildPath = Path.Combine(Path.GetTempPath(), "DefaultEngine.Editor");

        MSBuildWorkspace workspace = MSBuildWorkspace.Create();
    }

    private void Cleanup()
    {
        if (Directory.Exists(_buildPath))
        {
            Directory.Delete(_buildPath, true);
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        Cleanup();
    }
}
