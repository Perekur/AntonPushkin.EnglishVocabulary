using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PushkinA.EnglishVocabulary.Converters
{
    public class TestModeToWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2) return 0;

            var isTestMode = values[0] is bool ? (bool)values[0] : (bool)values[1];

            var actualWidth = values[0] is bool ? (double)values[1] : (double)values[0];

            return isTestMode ? 0 : actualWidth;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
