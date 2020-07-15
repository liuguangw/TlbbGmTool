using System;
using System.Globalization;
using System.Windows.Data;

namespace TlbbGmTool.Core.Convert
{
    public class IsLockToTipConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = value as bool?;
            var isLock = v ?? false;
            return isLock ? "已锁定" : "正常";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}