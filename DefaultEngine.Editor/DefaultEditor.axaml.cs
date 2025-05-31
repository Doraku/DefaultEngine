using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DefaultEngine.Editor.Services;
using DefaultEngine.Editor.ViewModels;
using DefaultUnDo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

namespace DefaultEngine.Editor;

public class DefaultEditor : Application, IDisposable
{
    private readonly List<IDisposable> _disposables;

    protected readonly Microsoft.Extensions.Logging.ILogger _logger;

    private bool _disposedValue;

    public DefaultEditor()
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Debug(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}",
                formatProvider: CultureInfo.InvariantCulture)
            .CreateLogger();

        _disposables = [];

        _logger = new SerilogLoggerProvider().CreateLogger("DefaultEditor");

        _logger.LogInformation("starting");
    }

    public override void Initialize()
    {
        base.Initialize();

        AvaloniaXamlLoader.Load(this);
    }

    private static void Main(string[] args) => Run<DefaultEditor>(args);

    private async Task<ServiceProvider> CreateServicesAsync()
    {
        await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);

        ServiceCollection services = new();

        RegisterServices(services);

        return services.BuildServiceProvider(true);
    }

    private async Task InitializeAsync(CancellationTokenSource shutdownTokenSource)
    {
        DefaultSplashScreen splashScreen = new();

        splashScreen.Show();

        await splashScreen.SetInformations("registering services").ConfigureAwait(true);

        ServiceProvider services = await CreateServicesAsync().ConfigureAwait(true);

        _disposables.Add(services);

        await splashScreen.SetInformations("building content").ConfigureAwait(true);

        object content = await CreateContentAsync(services).ConfigureAwait(true);

        await splashScreen.SetInformations("creating main window").ConfigureAwait(true);

        Window mainWindow = CreateMainWindow();

        mainWindow.Icon ??= splashScreen.Icon;
        mainWindow.Content = content;

        mainWindow.Closed += (_, _) => shutdownTokenSource.Cancel();
        mainWindow.Show();

        await splashScreen.SetInformations("welcome").ConfigureAwait(true);

        splashScreen.Close();
    }

    protected virtual void RegisterServices(IServiceCollection services)
    {
        services.AddLogging(builder => builder.AddSerilog(dispose: true));

        services.AddSingleton<IUnDoManager, UnDoManager>();

        services.AddSingleton<PluginsService>();

        services.AddSingleton<ShellViewModel>();
    }

    protected virtual async Task<object> CreateContentAsync(IServiceProvider services)
    {
        await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);

        return services.GetRequiredService<ShellViewModel>();
    }

    protected virtual Window CreateMainWindow()
        => new()
        {
            Title = "Default Engine",
            WindowState = WindowState.Maximized
        };

    public static void Run<T>(string[] args)
        where T : DefaultEditor, new()
    {
        static void Run(Application app, string[] args)
        {
            if (app is not T editor)
            {
                throw new InvalidOperationException();
            }

            using CancellationTokenSource shutdownTokenSource = new();

            using (editor)
            {
                _ = editor.InitializeAsync(shutdownTokenSource);

                editor.Run(shutdownTokenSource.Token);
            }
        }

        AppBuilder
            .Configure<T>()
            .UsePlatformDetect().Start(Run, args);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _disposables.Reverse();

                foreach (IDisposable disposable in _disposables)
                {
                    disposable.Dispose();
                }
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}