using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace PCController.Converters
{
    public class BooleanAndMultiConverter : IMultiValueConverter
    {
        #region Implementation of IMultiValueConverter

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var booleans = values.Cast<bool>();
            var result = booleans.All(o => o);
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
