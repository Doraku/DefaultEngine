using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using DefaultApplication.Controls.Metadata;
using DefaultApplication.DockingLayout.Internal.Controls;

namespace DefaultApplication.DockingLayout.Internal.Views;

[DataTemplate<SplitLayoutContent>]
public sealed partial class SplitLayoutContentView : Grid
{
    public SplitLayoutContentView()
    {
        InitializeComponent();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                default:
                    OnOrientationChanged(DataContext, new PropertyChangedEventArgs(nameof(SplitLayoutContent.Orientation)));
                    break;

            }
        }

        void OnOrientationChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is SplitLayoutContent content
                && e.PropertyName is nameof(SplitLayoutContent.Orientation))
            {
                Children.Clear();
                RowDefinitions.Clear();
                ColumnDefinitions.Clear();

                foreach (SplitLayoutItem item in content)
                {
                    LayoutContentPresenter presenter = new();

                    bool isFirst = Children.Count is 0;

                    GridSplitter? splitter = null;
                    if (!isFirst)
                    {
                        splitter = new()
                        {
                            ResizeBehavior = GridResizeBehavior.PreviousAndNext
                        };
                        Children.Add(splitter);
                    }

                    switch (content.Orientation)
                    {
                        case Orientation.Horizontal:
                            if (!isFirst)
                            {
                                SetColumn(splitter!, ColumnDefinitions.Count);
                                ColumnDefinitions.Add(new ColumnDefinition(new GridLength(5)));
                            }

                            SetColumn(presenter, ColumnDefinitions.Count);
                            ColumnDefinition column = new();
                            column.Bind(ColumnDefinition.WidthProperty, new Binding(nameof(item.Size), BindingMode.TwoWay) { Source = item });
                            ColumnDefinitions.Add(column);
                            break;

                        case Orientation.Vertical:
                            if (!isFirst)
                            {
                                SetRow(splitter!, RowDefinitions.Count);
                                RowDefinitions.Add(new RowDefinition(new GridLength(5)));
                            }

                            SetRow(presenter, RowDefinitions.Count);
                            RowDefinition row = new();
                            row.Bind(RowDefinition.HeightProperty, new Binding(nameof(item.Size), BindingMode.TwoWay) { Source = item });
                            RowDefinitions.Add(row);
                            break;
                    }

                    presenter.Bind(LayoutContentPresenter.ContentProperty, new Binding(nameof(item.Content), BindingMode.TwoWay) { Source = item });

                    Children.Add(presenter);
                }
            }
        }

        base.OnPropertyChanged(change);

        if (change?.Property != DataContextProperty)
        {
            return;
        }

        if (change.OldValue is SplitLayoutContent oldContent)
        {
            oldContent.CollectionChanged -= OnCollectionChanged;

            if (change.OldValue is INotifyPropertyChanged oldObservable)
            {
                oldObservable.PropertyChanged -= OnOrientationChanged;
            }
        }

        if (change.NewValue is SplitLayoutContent newContent)
        {
            newContent.CollectionChanged += OnCollectionChanged;

            if (change.NewValue is INotifyPropertyChanged newObservable)
            {
                newObservable.PropertyChanged += OnOrientationChanged;
            }

            OnOrientationChanged(newContent, new PropertyChangedEventArgs(nameof(SplitLayoutContent.Orientation)));
        }
    }
}