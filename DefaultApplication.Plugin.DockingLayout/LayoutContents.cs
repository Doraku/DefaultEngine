using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Layout;

namespace DefaultApplication.DockingLayout;

public interface ILayoutContent
{
    LayoutOptions Options { get; }

    object Content { get; }
}

public sealed class LayoutContent : ILayoutContent, INotifyPropertyChanged
{
    private LayoutOptions _options;
    private object _content;

    public LayoutOptions Options
    {
        get => _options;
        set => SetProperty(ref _options, value);
    }

    public object Content
    {
        get => _content;
        set => SetProperty(ref _content, value ?? throw new ArgumentNullException(nameof(value)));
    }

    public LayoutContent(LayoutOptions options, object content)
    {
        ArgumentNullException.ThrowIfNull(content);

        _options = options;
        _content = content;
    }

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    private void NotifyPropertyChanged(string? propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        field = value;

        NotifyPropertyChanged(propertyName);
    }

    #endregion
}

public sealed class SplitLayoutItem : INotifyPropertyChanged
{
    private ILayoutContent _content;
    private GridLength _size;

    public ILayoutContent Content
    {
        get => _content;
        set => SetProperty(ref _content, value ?? throw new ArgumentNullException(nameof(value)));
    }

    public GridLength Size
    {
        get => _size;
        set => SetProperty(ref _size, value);
    }

    public SplitLayoutItem(ILayoutContent content, GridLength size)
    {
        _content = content;
        _size = size;
    }

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    private void NotifyPropertyChanged(string? propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        field = value;

        NotifyPropertyChanged(propertyName);
    }

    #endregion
}

public sealed class SplitLayoutContent : ObservableCollection<SplitLayoutItem>, ILayoutContent
{
    private Orientation _orientation;

    public LayoutOptions Options => LayoutOptions.None;

    public object Content => this;

    public Orientation Orientation
    {
        get => _orientation;
        set => SetProperty(ref _orientation, value);
    }

    public SplitLayoutContent(Orientation orientation)
    {
        _orientation = orientation;
    }

    #region INotifyPropertyChanged

    private void NotifyPropertyChanged(string? propertyName) => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        field = value;

        NotifyPropertyChanged(propertyName);
    }

    #endregion
}

public sealed class StackedLayoutContent : ObservableCollection<ILayoutContent>, ILayoutContent
{
    public LayoutOptions Options => LayoutOptions.None;

    public object Content => this;
}