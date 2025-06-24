using System.Collections.Generic;

namespace DefaultApplication;

public interface ISettings
{
    IReadOnlyList<string> Path { get; }
}
