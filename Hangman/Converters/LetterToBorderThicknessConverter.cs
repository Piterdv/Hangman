using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hangman.Converters;

class LetterToBorderThicknessConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value as char? == ' ')
            return new Thickness(0, 0, 0, 0);

        return new Thickness(0, 0, 0, 2);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
