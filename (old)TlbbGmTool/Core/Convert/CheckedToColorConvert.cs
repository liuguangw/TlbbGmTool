using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TlbbGmTool.Core.Convert
{
    public class CheckedToColorConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = value as bool?;
            var isChecked = v ?? false;
            return isChecked ? Brushes.LightSkyBlue : Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}