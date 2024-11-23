using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data.Converters;
using Avalonia.Metadata;

namespace GameLibrary.UI.Controls;

public class GridView : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
        AvaloniaProperty.Register<GridView, IEnumerable?>(nameof(ItemsSource), coerce: ChunkItemsSource);

    public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty =
        ItemsControl.ItemTemplateProperty.AddOwner<GridView>();

    public static readonly StyledProperty<int> ColumnCountProperty =
        AvaloniaProperty.Register<GridView, int>(nameof(ColumnCount), defaultValue: 4);

    public IEnumerable? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    [InheritDataTypeFromItems(nameof(ItemsSource))]
    public IDataTemplate? ItemTemplate
    {
        get => GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public int ColumnCount
    {
        get => GetValue(ColumnCountProperty);
        set => SetValue(ColumnCountProperty, value);
    }

    private static IEnumerable? ChunkItemsSource(AvaloniaObject owner, IEnumerable? value)
    {
        Console.WriteLine(owner.GetValue(ColumnCountProperty));
        return value?.Cast<object?>().Chunk(owner.GetValue(ColumnCountProperty));
    }
}
