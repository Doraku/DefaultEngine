using System.Collections.Generic;

namespace DefaultApplication.Settings;

public interface ISettings
{
    IReadOnlyList<string> Path { get; }

    void Read();

    void Write();
}
