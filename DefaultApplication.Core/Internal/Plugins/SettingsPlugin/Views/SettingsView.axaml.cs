using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.VisualTree;
using DefaultApplication.Controls.Metadata;
using DefaultApplication.Internal.Plugins.SettingsPlugin.ViewModels;
using DefaultApplication.Settings;

namespace DefaultApplication.Internal.Plugins.SettingsPlugin.Views;

[DataTemplate<SettingsViewModel>]
internal sealed partial class SettingsView : DockPanel
{
    public sealed class SectionToIsVisibleConverter : IMultiValueConverter
    {
        private static readonly Dictionary<Type, string[]> _settingsStrings = [];

        private readonly Application _application;

        public static bool Convert(Application application, ISettings settings, string? filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return true;
            }

            if (!_settingsStrings.TryGetValue(settings.GetType(), out string[]? strings))
            {
                strings =
                    application.DataTemplates.FirstOrDefault(template => template.Match(settings))?.Build(settings) is Control view
                    ? [.. view.GetVisualDescendants()
                        .OfType<SelectableTextBlock>()
                        .Select(textBlock => textBlock.Text)
                        .Where(text => !string.IsNullOrEmpty(text))
                        .Select(text => text!)]
                    : [];
                _settingsStrings.Add(settings.GetType(), strings);
            }

            return strings.Any(value => value.Contains(filter, StringComparison.OrdinalIgnoreCase));
        }

        private bool Convert(SectionViewModel section, string? filter)
            => string.IsNullOrEmpty(filter)
            || (section.Settings?.Any(settings => Convert(_application, settings, filter)) ?? false)
            || (section.Sections?.Any(subSection => Convert(subSection, filter)) ?? false);

        public SectionToIsVisibleConverter(Application application)
        {
            _application = application;
        }

        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
            => values.Count is 2
            && values[0] is SectionViewModel section
            && Convert(section, values[1] as string);
    }

    public sealed class SectionToSettingsConverter : IMultiValueConverter
    {
        private readonly Application _application;

        private IEnumerable<ISettings> Convert(SectionViewModel section, string? filter)
        {
            IEnumerable<ISettings> shownSettings = section.Settings?.Where(settings => SectionToIsVisibleConverter.Convert(_application, settings, filter)) ?? [];

            if (!shownSettings.Any())
            {
                shownSettings = section.Sections?.Select(subSection => Convert(subSection, filter)).FirstOrDefault(settings => settings.Any()) ?? [];
            }

            return shownSettings;
        }

        public SectionToSettingsConverter(Application application)
        {
            _application = application;
        }

        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
            => values.Count is 2
            && values[0] is SectionViewModel section
            ? Convert(section, values[1] as string)
            : null;
    }

    public SettingsView(Application application)
    {
        Resources["SectionToIsVisibleConverter"] = new SectionToIsVisibleConverter(application);
        Resources["SectionToSettingsConverter"] = new SectionToSettingsConverter(application);

        InitializeComponent();
    }

    private async void OnSettingsPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (sender is not ItemsControl control
            || e.Property != ItemsControl.ItemsSourceProperty)
        {
            return;
        }

        await Task.Delay(TimeSpan.FromMilliseconds(100)).ConfigureAwait(true);

        foreach (SelectableTextBlock block in control.GetVisualDescendants().OfType<SelectableTextBlock>())
        {
            if (Filter.Text is string filter && (block.Text?.Contains(filter, StringComparison.OrdinalIgnoreCase) ?? false))
            {
                block.SelectionStart = block.Text.IndexOf(filter, StringComparison.OrdinalIgnoreCase);
                block.SelectionEnd = block.SelectionStart + filter.Length;
            }
            else
            {
                block.ClearSelection();
            }
        }
    }
}