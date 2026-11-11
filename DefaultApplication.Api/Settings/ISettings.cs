using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DefaultApplication.Settings;

public interface ISettings
{
    IReadOnlyList<string> Path { get; }

    Task ReadAsync(CancellationToken cancellationToken);

    Task WriteAsync(CancellationToken cancellationToken);
}
