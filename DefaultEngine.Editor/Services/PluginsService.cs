using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using DefaultEngine.Editor.Api;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace DefaultEngine.Editor.Services;

internal sealed class PluginsService : IDisposable
{
    private readonly string _buildPath;

    private WeakReference? _pluginsReference;

    public PluginsService()
    {
        _buildPath = Path.Combine(Path.GetTempPath(), "DefaultEngine.Editor");
    }

    private void Cleanup()
    {
        if (Directory.Exists(_buildPath))
        {
            Directory.Delete(_buildPath, true);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Unload()
    {
        (_pluginsReference?.Target as AssemblyLoadContext)?.Unload();
    }

    public async Task Load()
    {
        Unload();

        await Task.Delay(1000).ConfigureAwait(false);

        while (_pluginsReference?.IsAlive is true)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        //Cleanup();

        using MSBuildWorkspace workspace = MSBuildWorkspace.Create();

        Project project = await workspace.OpenProjectAsync("D:\\Projects\\DefaultEngine\\Test\\Test.csproj").ConfigureAwait(false);
        //Compilation? compilation = await project.GetCompilationAsync().ConfigureAwait(false);

        if (project.OutputFilePath is not null)
        {
            DirectoryInfo baseBuild = new(Path.GetDirectoryName(project.OutputFilePath)!);

pouet:
            try
            {
                baseBuild.Delete(true);
            }
            catch (UnauthorizedAccessException)
            {
                goto pouet;
            }

            while (baseBuild.Exists)
            {
                await Task.Delay(100).ConfigureAwait(false);
            }

            await Process.Start("dotnet", ["build", project.FilePath!]).WaitForExitAsync().ConfigureAwait(false);

            AssemblyLoadContext plugins = new("Plugins", true);
            _pluginsReference = new WeakReference(plugins, true);

            Assembly assembly = plugins.LoadFromAssemblyPath(project.OutputFilePath);

            foreach (EditorMenuAttribute attribute in assembly.GetTypes().SelectMany(type => type.GetRuntimeMethods().Select(method => method.GetCustomAttribute<EditorMenuAttribute>())).Where(attribute => attribute is not null)!)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    new Window()
                    {
                        Content = attribute.DisplayName
                    }.Show();
                });
            }

        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        Cleanup();
    }
}
