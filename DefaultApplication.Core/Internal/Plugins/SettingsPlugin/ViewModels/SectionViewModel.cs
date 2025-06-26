using System;
using System.Collections.Generic;
using System.Linq;

namespace DefaultApplication.Internal.Plugins.SettingsPlugin.ViewModels;

internal sealed class SectionViewModel
{
    private List<SectionViewModel>? _sections;
    private List<ISettings>? _settings;

    public string Header { get; }

    public IReadOnlyList<SectionViewModel>? Sections => _sections;

    public IEnumerable<ISettings> Settings => _settings ?? Sections?.Select(section => section.Settings).FirstOrDefault() ?? [];

    public SectionViewModel(string header)
    {
        Header = header;
    }

    public static int Compare(SectionViewModel section1, SectionViewModel section2)
    {
        return string.Compare(section1.Header, section2.Header, StringComparison.OrdinalIgnoreCase);
    }

    public void Add(SectionViewModel section)
    {
        _sections ??= [];
        _sections.Add(section);
        _sections.Sort(Compare);
    }

    public void Add(ISettings settings)
    {
        _settings ??= [];
        _settings.Add(settings);
    }
}
