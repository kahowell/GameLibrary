using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace GameLibrary.UI.Controls;

public class ChunkConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        Console.WriteLine($"value: {value}");
        TemplateBinding b;
        //b.ProvideValue();
        Console.WriteLine(parameter);
        var columns = (int)parameter!;
        var enumerable = (IEnumerable<object?>)value!;
        return enumerable.Chunk(columns);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
