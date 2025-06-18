using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DefaultEngine.Editor.Api.Plugins;
using DefaultEngine.Editor.Internal;
using DefaultEngine.Editor.Internal.Plugins.ShellPlugin.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

        AppDomain.CurrentDomain.UnhandledException += (_, args) => _logger.LogError(args.ExceptionObject as Exception, "Unhandled exception");
        TaskScheduler.UnobservedTaskException += (_, args) => _logger.LogError(args.Exception, "Unobserved task exception");

        _logger.LogInformation("starting");
    }

    public override void Initialize()
    {
        base.Initialize();

        AvaloniaXamlLoader.Load(this);
    }

    private async Task<IServiceProvider> CreateServicesAsync()
    {
        await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);

        PluginsHelper plugins = new(new FileInfo(Assembly.GetEntryAssembly()!.Location).Directory!);

        ServiceProviderOptions options = new()
        {
            ValidateOnBuild = true,
            ValidateScopes = true
        };

        ServiceCollection pluginsServices = [];

        pluginsServices.AddLogging(builder => builder.AddSerilog(dispose: true));
        pluginsServices.AddSingleton(plugins);
        pluginsServices.AddSingleton<Application>(this);

        foreach (Type type in plugins.GetPluginsTypes().GetInstanciableImplementation<IServicesRegisterer>())
        {
            pluginsServices.TryAddSingleton(type);
            pluginsServices.AddSingleton(typeof(IServicesRegisterer), provider => provider.GetRequiredService(type));
        }

        ServiceProvider pluginsProvider = pluginsServices.BuildServiceProvider(options);

        _disposables.Add(pluginsProvider);

        ServiceCollection services = [];

        services.AddLogging(builder => builder.AddSerilog(dispose: true));
        services.AddSingleton<Application>(this);

        foreach (IServicesRegisterer servicesRegisterer in pluginsProvider.GetRequiredService<IEnumerable<IServicesRegisterer>>())
        {
            servicesRegisterer.Register(services);
        }

        ServiceProvider provider = services.BuildServiceProvider(options);

        _disposables.Add(provider);

        provider.GetService<IEnumerable<IPlugin>>();

        return provider;
    }

    private async Task InitializeAsync(CancellationTokenSource shutdownTokenSource)
    {
        try
        {
            DefaultSplashScreen splashScreen = new(_logger);

            splashScreen.Show();

            await splashScreen.SetInformations("registering services").ConfigureAwait(true);

            IServiceProvider services = await CreateServicesAsync().ConfigureAwait(true);

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
        catch (Exception exception)
        {
            _logger.LogCritical(exception, "Error during initialization");

            await shutdownTokenSource.CancelAsync().ConfigureAwait(true);

            throw;
        }
    }

    protected virtual async Task<object> CreateContentAsync(IServiceProvider services)
    {
        await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);

        return services.GetRequiredService<ShellViewModel>();
    }

    protected virtual Window CreateMainWindow()
    {
        Window window = new()
        {
            Title = "Default Engine",
            WindowState = WindowState.Maximized,
            ExtendClientAreaToDecorationsHint = true
        };

#if DEBUG
        window.AttachDevTools();
#endif

        return window;
    }

    public static void Run<T>(string[] args)
        where T : DefaultEditor, new()
    {
        static void Run(Application app, string[] args)
        {
            if (app is not T editor)
            {
                throw new InvalidOperationException();
            }

            editor._logger.LogInformation($"args {args}");

            using CancellationTokenSource shutdownTokenSource = new();

            using (editor)
            {
                _ = editor.InitializeAsync(shutdownTokenSource);

                editor.Run(shutdownTokenSource.Token);
            }
        }

        AppBuilder
            .Configure<T>()
            .UsePlatformDetect()
            .LogToTrace()
            .Start(Run, args);
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