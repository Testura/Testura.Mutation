using System;
using System.Globalization;
using System.Windows.Data;

namespace Testura.Mutation.Wpf.Shared.Converters
{
    public class IsZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value == 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
