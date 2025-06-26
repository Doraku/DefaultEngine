using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DefaultApplication.Services;

public static class IMenuServiceExtensions
{
    public static bool TryGet(this IMenuService service, [NotNullWhen(true)] out IMenuService.IMenuCommand? command, params ReadOnlySpan<string> path)
    {
        ArgumentNullException.ThrowIfNull(service);

        command = null;

        foreach (ref readonly string header in path)
        {
            string searchedHeader = header;
            command = (command?.SubCommands ?? service.Commands).FirstOrDefault(subMenu => string.Equals(subMenu.Header, searchedHeader, StringComparison.OrdinalIgnoreCase));

            if (command is null)
            {
                return false;
            }
        }

        return command != null;
    }
}
