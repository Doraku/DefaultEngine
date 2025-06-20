using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;

namespace DefaultEngine.Editor.Api.Controls.Templates;

public sealed class DataTemplateInclude : IDataTemplate
{
    private readonly Uri? _baseUri;
    private DataTemplates? _loaded;
    private bool _isLoading;

    public Uri? Source { get; set; }

    public DataTemplates? Loaded
    {
        get
        {
            if (_loaded is null && Source is { })
            {
                _isLoading = true;
                _loaded = (DataTemplates)AvaloniaXamlLoader.Load(Source, _baseUri);
                _isLoading = false;
            }

            return _loaded;
        }
    }

    public DataTemplateInclude(Uri? baseUri)
    {
        _baseUri = baseUri;
    }

    public DataTemplateInclude(IServiceProvider serviceProvider)
        : this(serviceProvider.GetService<IUriContext>()?.BaseUri)
    { }

    public bool Match(object? data)
    {
        return !_isLoading && (Loaded?.Any(dt => dt.Match(data)) ?? false);
    }

    public Control? Build(object? data)
    {
        return _isLoading ? null : Loaded?.FirstOrDefault(dt => dt.Match(data))?.Build(data);
    }
}
