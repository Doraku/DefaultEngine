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
public partial class SplitLayoutContentView : Panel
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
                ItemsGrid.Children.Clear();
                ItemsGrid.RowDefinitions.Clear();
                ItemsGrid.ColumnDefinitions.Clear();

                foreach (SplitLayoutItem item in content)
                {
                    LayoutContentPresenter presenter = new();

                    bool isFirst = ItemsGrid.Children.Count is 0;

                    GridSplitter? splitter = null;
                    if (!isFirst)
                    {
                        splitter = new()
                        {
                            ResizeBehavior = GridResizeBehavior.PreviousAndNext
                        };
                        ItemsGrid.Children.Add(splitter);
                    }

                    switch (content.Orientation)
                    {
                        case Orientation.Horizontal:
                            if (!isFirst)
                            {
                                Grid.SetColumn(splitter!, ItemsGrid.ColumnDefinitions.Count);
                                ItemsGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(5)));
                            }

                            Grid.SetColumn(presenter, ItemsGrid.ColumnDefinitions.Count);
                            ColumnDefinition column = new();
                            column.Bind(ColumnDefinition.WidthProperty, new Binding(nameof(item.Size), BindingMode.TwoWay) { Source = item });
                            ItemsGrid.ColumnDefinitions.Add(column);
                            break;

                        case Orientation.Vertical:
                            if (!isFirst)
                            {
                                Grid.SetRow(splitter!, ItemsGrid.RowDefinitions.Count);
                                ItemsGrid.RowDefinitions.Add(new RowDefinition(new GridLength(5)));
                            }

                            Grid.SetRow(presenter, ItemsGrid.RowDefinitions.Count);
                            RowDefinition row = new();
                            row.Bind(RowDefinition.HeightProperty, new Binding(nameof(item.Size), BindingMode.TwoWay) { Source = item });
                            ItemsGrid.RowDefinitions.Add(row);
                            break;
                    }

                    presenter.Bind(LayoutContentPresenter.ContentProperty, new Binding(nameof(item.Content), BindingMode.TwoWay) { Source = item });

                    ItemsGrid.Children.Add(presenter);
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