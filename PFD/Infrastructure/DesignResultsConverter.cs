using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace PFD
{
    class DesignResultsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            float resValue = (float)value;

            //bool revert = (parameter as string).StartsWith("-");

            string stringValue = value as string;
            string compareValue = parameter as string;

            if (resValue > 1f) return new SolidColorBrush(Colors.Red); // return "Bad"; 
            else if (resValue < 0.3f) return new SolidColorBrush(Colors.LightGray); //return "Neutral";
            else return new SolidColorBrush(Colors.GreenYellow); //return "Good";
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    
}
