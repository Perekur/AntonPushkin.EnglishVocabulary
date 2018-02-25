using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PushkinA.EnglishVocabulary.Converters
{
    public class TestModeToWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
                var actualWidth = (double)values[0];
                var isVisible1 = values[1] == DependencyProperty.UnsetValue ? false : (bool)values[1];
                var isVisible2 = values.Length==2 ? true : values[2] == DependencyProperty.UnsetValue ? false : (bool)values[2];

                return isVisible1 && isVisible2 ? actualWidth: 0;
            }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
