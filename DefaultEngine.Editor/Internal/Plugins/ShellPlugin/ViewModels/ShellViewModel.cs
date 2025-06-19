using System.Collections.Generic;
using System.Linq;
using DefaultEngine.Editor.Api;
using Microsoft.Extensions.Logging;

namespace DefaultEngine.Editor.Internal.Plugins.ShellPlugin.ViewModels;

internal sealed class ShellViewModel
{
    private readonly List<MenuViewModel> _menus;

    public IReadOnlyCollection<MenuViewModel> Menus => _menus;

    public ShellViewModel(ILogger<ShellViewModel> logger, IEnumerable<IMenu> menus)
    {
        _menus = [];

        Dictionary<string, IMenu> distinctMenus = [];

        static string GetKey(IEnumerable<string> path) => string.Join('>', path);

        foreach (IMenu menu in menus)
        {
            string key = GetKey(menu.Path);

            if (menu is not IBaseCommandMenu)
            {
                key += '>';
            }

            if (distinctMenus.TryGetValue(key, out IMenu? firstMenu))
            {
                logger.LogWarning($"ignoring {menu.GetType()} at {key}, {firstMenu.GetType()} already present");

                continue;
            }

            distinctMenus.Add(key, menu);
        }

        Dictionary<string, MenuViewModel> viewModels = [];

        while (distinctMenus.Count > 0)
        {
            string key = distinctMenus.Keys.First();
            if (distinctMenus.Remove(key, out IMenu? value))
            {
                MenuViewModel viewModel = new(value);
                viewModels.Add(key, viewModel);
                bool isNew = true;

                for (int i = value.Path.Count - 2; i >= 0; --i)
                {
                    string parentKey = GetKey(value.Path.Take(i + 1)) + '>';
                    isNew = !viewModels.TryGetValue(parentKey, out MenuViewModel? parentViewModel);

                    if (isNew)
                    {
                        parentViewModel = distinctMenus.TryGetValue(parentKey, out IMenu? parentMenu) ? new MenuViewModel(parentMenu) : new MenuViewModel(value.Path[i]);
                        viewModels.Add(parentKey, parentViewModel);
                    }

                    parentViewModel!.Add(viewModel);
                    viewModel = parentViewModel;
                }

                if (isNew)
                {
                    _menus.Add(viewModel);
                }
            }
        }

        _menus.Sort(MenuViewModel.Compare);
    }
}
