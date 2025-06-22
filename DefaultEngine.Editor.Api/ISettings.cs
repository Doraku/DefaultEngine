using System.Collections.Generic;

namespace DefaultEngine.Editor.Api;

public interface ISettings
{
    IReadOnlyList<string> Path { get; }
}
