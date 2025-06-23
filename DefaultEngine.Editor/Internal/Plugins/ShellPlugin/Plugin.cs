using System;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Threading;
using DefaultEngine.Editor.Api;
using DefaultEngine.Editor.Api.Plugins;
using DefaultEngine.Editor.Api.Services;
using DefaultEngine.Editor.Internal.Plugins.ShellPlugin.ViewModels;
using DefaultEngine.Editor.Internal.Plugins.ShellPlugin.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultEngine.Editor.Internal.Plugins.ShellPlugin;

internal sealed class Plugin : IServicesRegisterer
{
    private readonly PluginsHelper _plugins;

    public Plugin(Application application, PluginsHelper pluginsHelper)
    {
        Uri baseUri = new("avares://DefaultEngine.Editor");
        Uri resourcesUri = new(baseUri, "Internal/Plugins/ShellPlugin/Resources/");

        Dispatcher.UIThread.Invoke(() => application.Resources.MergedDictionaries.Add(new ResourceInclude(baseUri) { Source = new Uri(resourcesUri, "Resources.axaml") }));

        _plugins = pluginsHelper;
    }

    public void Register(IServiceCollection services)
    {
        services.AddDelayedSingleton(out TaskCompletionSource<Window> delayedMainWindow);

        services.TryAddSingleton<ShellViewModel>();
        services.TryAddSingleton<IContentDialogService>(provider =>
        {
            ShellView instance = Dispatcher.UIThread.Invoke(provider.GetRequiredService<ShellView>);

            instance.Loaded += (_, _) => _ = TopLevel.GetTopLevel(instance) is Window window && delayedMainWindow.TrySetResult(window);

            return instance;
        });

        services.TryAddTransient<AboutViewModel>();

        foreach (Type type in _plugins.GetPluginsTypes().GetInstanciableImplementation<IMenu>())
        {
            services.AddAsSingletonImplementation<IMenu>(type);
        }
    }
}
