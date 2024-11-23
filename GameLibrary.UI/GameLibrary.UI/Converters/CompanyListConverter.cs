using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data;
using Avalonia.Data.Converters;
using GameLibrary.Core.Models;

namespace GameLibrary.UI.Converters;

public class CompanyListConverter: IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            null => null,
            IEnumerable<Company> companies => string.Join(", ", companies.Select(company => company.Name)),
            _ => new BindingNotification(new ArgumentException($"{value} is not a company list."),
                BindingErrorType.Error)
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return new BindingNotification(new NotSupportedException(), BindingErrorType.Error);
    }
}
