using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Threading;
using DefaultApplication.ComponentModel;
using DefaultApplication.Internal.Plugins.SettingsPlugin.ViewModels;
using DefaultApplication.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultApplication.Internal.Plugins.SettingsPlugin;

internal sealed class Plugin : IPlugin
{
    private sealed class Registerer : IServicesRegisterer
    {
        private readonly PluginsHelper _plugins;

        public Registerer(PluginsHelper pluginsHelper)
        {
            _plugins = pluginsHelper;
        }

        public void Register(IServiceCollection services)
        {
            services.TryAddTransient<SettingsViewModel>();

            foreach (Type type in _plugins.GetPluginsTypes().GetInstanciableImplementation<ISettings>())
            {
                services.AddAsSingletonImplementation<ISettings>(type);
            }
        }
    }

    public Plugin(Application application, IEnumerable<ISettings> settings)
    {
        Uri baseUri = new("avares://DefaultApplication.Core");
        Uri resourcesUri = new(baseUri, "Internal/Plugins/SettingsPlugin/Resources/");

        Dispatcher.UIThread.Invoke(() => application.Resources.MergedDictionaries.Add(new ResourceInclude(baseUri) { Source = new Uri(resourcesUri, "Resources.axaml") }));

        foreach (Type settingsType in settings.Select(part => part.GetType()))
        {
            application.DataTemplates.Add(new FuncDataTemplate(settingsType, (_, _) =>
            {
                Grid grid = new()
                {
                    ColumnSpacing = 5,
                    RowSpacing = 5,
                    Margin = new Thickness(0, 5, 0, 0)
                };
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto) { SharedSizeGroup = "SettingsHeaders" });
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

                foreach (PropertyInfo? property in settingsType
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(property => property.GetGetMethod() is { } && property.GetSetMethod() is { }))
                {
                    grid.Children.Add(new TextBlock
                    {
                        Text = property.GetCustomAttribute<DescriptionAttribute>()?.Description ?? property.Name,
                        VerticalAlignment = VerticalAlignment.Center,
                        [Grid.ColumnProperty] = 0,
                        [Grid.RowProperty] = grid.RowDefinitions.Count,
                    });

                    Control? value = null;

                    if (property.GetCustomAttribute<ItemsSourceAttribute>() is ItemsSourceAttribute itemsSource)
                    {
                        MethodInfo member = settingsType.GetProperty(itemsSource.MemberName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)?.GetGetMethod() ?? throw new Exception();

                        value = new ComboBox
                        {
                            [!SelectingItemsControl.SelectedItemProperty] = new Binding(property.Name)
                        };

                        if (member.IsStatic)
                        {
                            value[ItemsControl.ItemsSourceProperty] = member.Invoke(null, null);
                        }
                        else
                        {
                            value[!ItemsControl.ItemsSourceProperty] = new Binding(itemsSource.MemberName);
                        }
                    }
                    else if (property.PropertyType == typeof(string))
                    {
                        value = new TextBox
                        {
                            [!TextBox.TextProperty] = new Binding(property.Name)
                        };
                    }
                    else if (property.PropertyType == typeof(bool))
                    {
                        value = new CheckBox
                        {
                            [!ToggleButton.IsCheckedProperty] = new Binding(property.Name)
                        };
                    }

                    if (value is { })
                    {
                        value[Grid.ColumnProperty] = 1;
                        value[Grid.RowProperty] = grid.RowDefinitions.Count;
                        grid.Children.Add(value);
                    }

                    grid.RowDefinitions.Add(new RowDefinition());
                }

                return grid;
            }));
        }
    }
}