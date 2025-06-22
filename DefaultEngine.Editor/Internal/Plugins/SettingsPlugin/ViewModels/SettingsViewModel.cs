using System.Collections.Generic;
using System.Linq;
using DefaultEngine.Editor.Api;
using Microsoft.Extensions.Logging;

namespace DefaultEngine.Editor.Internal.Plugins.SettingsPlugin.ViewModels;

internal sealed class SettingsViewModel
{
    private readonly List<SectionViewModel> _sections;

    public IReadOnlyCollection<SectionViewModel> Sections => _sections;

    public SettingsViewModel(ILogger<SettingsViewModel> logger, IEnumerable<ISettings> settings)
    {
        _sections = [];

        Dictionary<string, SectionViewModel> distinctSections = [];

        static string GetKey(IEnumerable<string> path) => string.Join('>', path);

        foreach (ISettings settingsPart in settings)
        {
            SectionViewModel? parentSection = null;

            for (int i = 0; i < settingsPart.Path.Count; ++i)
            {
                string sectionKey = GetKey(settingsPart.Path.Take(i + 1));

                if (!distinctSections.TryGetValue(sectionKey, out SectionViewModel? section))
                {
                    section = new SectionViewModel(settingsPart.Path[i]);
                }

                if (parentSection is { })
                {
                    parentSection.Add(section);
                }
                else
                {
                    _sections.Add(section);
                }

                parentSection = section;
            }

            if (parentSection is null)
            {
                logger.LogWarning($"ignoring {settingsPart.GetType()} as no path was provided");
                continue;
            }

            parentSection.Add(settingsPart);
        }

        _sections.Sort(SectionViewModel.Compare);
    }
}
