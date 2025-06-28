using System;
using System.Threading.Tasks;

namespace DefaultApplication;

public interface ISplashScreen : IDisposable
{
    Task ReportAsync(string message);
}
