using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Input;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Threading;
using DefaultEngine.Editor.Api.Plugins;
using DefaultUnDo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DefaultEngine.Editor.Internal.Plugins;

internal sealed class UnDoPlugin : IServicesRegisterer
{
    internal sealed class Undo : IMenuItem
    {
        private readonly IUnDoManager _manager;

        public IReadOnlyList<string> Path { get; } = ["Edit", "Undo"];

        public object? Icon { get; } = new StaticResourceExtension("UndoGeometry");

        public KeyGesture HotKey { get; } = new(Key.Z, KeyModifiers.Control);

        public Undo(IUnDoManager manager)
        {
            _manager = manager;
        }

        public bool CanExecute() => _manager.CanUndo;

        public void Execute() => _manager.Undo();
    }

    internal sealed class Redo : IMenuItem
    {
        private readonly IUnDoManager _manager;

        public IReadOnlyList<string> Path { get; } = ["Edit", "Redo"];

        public KeyGesture HotKey { get; } = new(Key.Y, KeyModifiers.Control);

        public Redo(IUnDoManager manager)
        {
            _manager = manager;
        }

        public bool CanExecute() => _manager.CanRedo;

        public void Execute() => _manager.Redo();
    }

    public UnDoPlugin(Application application)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            application.Styles.Add(new StyleInclude(new Uri("avares://DefaultEngine.Editor")) { Source = new Uri("avares://DefaultEngine.Editor/Resources/Styles/UnDoOverrides.axaml") });

            application.Resources.MergedDictionaries.Add(new ResourceInclude(new Uri("avares://DefaultEngine.Editor")) { Source = new Uri("avares://DefaultEngine.Editor/Resources/UnDoResources.axaml") });
        });
    }

    public void Register(IServiceCollection services) => services.TryAddSingleton<IUnDoManager, UnDoManager>();
}
