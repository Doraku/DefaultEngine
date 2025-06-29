using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using DefaultApplication.ComponentModel;

namespace DefaultApplication.Internal.Plugins.SettingsPlugin.Controls.Templates;

internal sealed class SettingsTemplate : IDataTemplate
{
    private static readonly Dictionary<Type, Func<IBinding, Control>> _factories = new()
    {
        [typeof(string)] = binding => new TextBox { [!TextBox.TextProperty] = binding },
        [typeof(bool)] = binding => new CheckBox { [!ToggleButton.IsCheckedProperty] = binding },
        [typeof(byte)] = binding => CreateNumericUpDown(byte.MinValue, byte.MaxValue, NumberStyles.Integer, binding),
        [typeof(sbyte)] = binding => CreateNumericUpDown(sbyte.MinValue, sbyte.MaxValue, NumberStyles.Integer, binding),
        [typeof(ushort)] = binding => CreateNumericUpDown(ushort.MinValue, ushort.MaxValue, NumberStyles.Integer, binding),
        [typeof(short)] = binding => CreateNumericUpDown(short.MinValue, short.MaxValue, NumberStyles.Integer, binding),
        [typeof(uint)] = binding => CreateNumericUpDown(uint.MinValue, uint.MaxValue, NumberStyles.Integer, binding),
        [typeof(int)] = binding => CreateNumericUpDown(int.MinValue, int.MaxValue, NumberStyles.Integer, binding),
        [typeof(ulong)] = binding => CreateNumericUpDown(ulong.MinValue, ulong.MaxValue, NumberStyles.Integer, binding),
        [typeof(long)] = binding => CreateNumericUpDown(long.MinValue, long.MaxValue, NumberStyles.Integer, binding),
        [typeof(float)] = binding => CreateNumericUpDown(decimal.MinValue, decimal.MaxValue, NumberStyles.Float, binding),
        [typeof(double)] = binding => CreateNumericUpDown(decimal.MinValue, decimal.MaxValue, NumberStyles.Float, binding),
        [typeof(decimal)] = binding => CreateNumericUpDown(decimal.MinValue, decimal.MaxValue, NumberStyles.Float, binding),
    };

    private readonly Type _settingsType;
    private readonly List<(string Name, IBinding ValueBinding, Func<IBinding, Control> ControlFactory)> _members;

    public SettingsTemplate(Type settingsType)
    {
        _settingsType = settingsType;
        _members = [];

        foreach (PropertyInfo? property in _settingsType
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetGetMethod() is { } && property.GetSetMethod() is { }))
        {
            string name = property.GetCustomAttribute<DescriptionAttribute>()?.Description ?? property.Name;
            Binding valueBinding = new(property.Name) { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };

            if (property.GetCustomAttribute<SettingsItemsSourceAttribute>() is SettingsItemsSourceAttribute itemsSource)
            {
                MethodInfo? itemsSourceGetter = _settingsType.GetProperty(itemsSource.MemberName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)?.GetGetMethod();

                if (itemsSourceGetter is null)
                { }
                else if (itemsSourceGetter.IsStatic)
                {
                    _members.Add((name, valueBinding, binding => new ComboBox
                    {
                        [!SelectingItemsControl.SelectedItemProperty] = binding,
                        [ItemsControl.ItemsSourceProperty] = itemsSourceGetter.Invoke(null, null)
                    }));
                }
                else
                {
                    _members.Add((name, valueBinding, binding => new ComboBox
                    {
                        [!SelectingItemsControl.SelectedItemProperty] = binding,
                        [!ItemsControl.ItemsSourceProperty] = new Binding(itemsSource.MemberName)
                    }));
                }
            }
            else if (_factories.TryGetValue(property.PropertyType, out Func<IBinding, Control>? factory))
            {
                _members.Add((name, valueBinding, factory));
            }
        }
    }

    private static NumericUpDown CreateNumericUpDown(decimal minimum, decimal maximum, NumberStyles parsingNumberStyle, IBinding valueBinding)
        => new()
        {
            ShowButtonSpinner = false,
            Minimum = minimum,
            Maximum = maximum,
            ParsingNumberStyle = parsingNumberStyle,
            [!NumericUpDown.ValueProperty] = valueBinding
        };

    public Control? Build(object? param)
    {
        Grid grid = new()
        {
            ColumnSpacing = 5,
            RowSpacing = 5,
            Margin = new Thickness(0, 5, 0, 0)
        };
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto) { SharedSizeGroup = "SettingsHeaders" });
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

        foreach ((string name, IBinding valueBinding, Func<IBinding, Control> controlFactory) in _members)
        {
            grid.Children.Add(new SelectableTextBlock
            {
                Text = name,
                VerticalAlignment = VerticalAlignment.Center,
                IsTabStop = false,
                IsEnabled = false,
                [Grid.ColumnProperty] = 0,
                [Grid.RowProperty] = grid.RowDefinitions.Count,
            });

            Control valueControl = controlFactory(valueBinding);
            valueControl[Grid.ColumnProperty] = 1;
            valueControl[Grid.RowProperty] = grid.RowDefinitions.Count;

            grid.Children.Add(valueControl);

            grid.RowDefinitions.Add(new RowDefinition());
        }

        return grid;
    }

    public bool Match(object? data) => _settingsType.IsInstanceOfType(data);
}
