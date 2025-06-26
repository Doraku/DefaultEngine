using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace DefaultApplication.Internal.Plugins.SettingsPlugin.ViewModels;

internal sealed class SettingsViewModel
{
    public SectionViewModel RootSection { get; }

    public SettingsViewModel(ILogger<SettingsViewModel> logger, IEnumerable<ISettings> settings)
    {
        RootSection = new SectionViewModel(string.Empty);

        Dictionary<string, SectionViewModel> distinctSections = [];

        static string GetKey(IEnumerable<string> path) => string.Join('>', path);

        foreach (ISettings settingsPart in settings)
        {
            if (settingsPart.Path is null || settingsPart.Path.Count is 0)
            {
                logger.LogIgnoringEmptyPathSettings(settingsPart);
                continue;
            }

            SectionViewModel parentSection = RootSection;

            for (int i = 0; i < settingsPart.Path.Count; ++i)
            {
                string sectionKey = GetKey(settingsPart.Path.Take(i + 1));

                if (!distinctSections.TryGetValue(sectionKey, out SectionViewModel? section))
                {
                    section = new SectionViewModel(settingsPart.Path[i]);
                    distinctSections.Add(sectionKey, section);

                    parentSection.Add(section);
                }

                parentSection = section;
            }

            parentSection.Add(settingsPart);
        }
    }
}
