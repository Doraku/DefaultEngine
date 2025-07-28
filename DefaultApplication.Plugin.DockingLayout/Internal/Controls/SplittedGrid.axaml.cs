using System.Collections.Generic;
using Avalonia.Controls;

namespace DefaultApplication.DockingLayout.Internal.Controls;

internal enum SplittedGridMode
{
    AllRelative,
    FirstFill,
    SecondFill
}

internal sealed partial class SplittedGrid : Grid
{
    public SplittedGrid()
    {
        InitializeComponent();
    }

    private static IEnumerable<ColumnDefinition> GetColumns(SplittedGridMode mode)
    {
        yield return new ColumnDefinition(mode switch
        {
            SplittedGridMode.SecondFill => GridLength.Auto,
            _ => GridLength.Star
        });
        yield return new ColumnDefinition(new GridLength(5));
        yield return new ColumnDefinition(mode switch
        {
            SplittedGridMode.FirstFill => GridLength.Auto,
            _ => GridLength.Star
        });
    }

    private static IEnumerable<RowDefinition> GetRows(SplittedGridMode mode)
    {
        yield return new RowDefinition(mode switch
        {
            SplittedGridMode.SecondFill => GridLength.Auto,
            _ => GridLength.Star
        });
        yield return new RowDefinition(new GridLength(5));
        yield return new RowDefinition(mode switch
        {
            SplittedGridMode.FirstFill => GridLength.Auto,
            _ => GridLength.Star
        });
    }

    public static SplittedGrid CreateHorizontal(object firstContent, object secondContent, SplittedGridMode mode)
    {
        SplittedGrid control = new()
        {
            ColumnDefinitions = [.. GetColumns(mode)]
        };

        control.FirstContent.Content = firstContent;
        control.SecondContent.Content = secondContent;

        return control;
    }

    public static SplittedGrid CreateVertical(object firstContent, object secondContent, SplittedGridMode mode)
    {
        SplittedGrid control = new()
        {
            RowDefinitions = [.. GetRows(mode)]
        };

        control.FirstContent.Content = firstContent;
        control.SecondContent.Content = secondContent;

        return control;
    }
}