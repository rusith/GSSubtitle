using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GSSubtitle.Tools.SubtitleTools.ValueConverters
{
    /// <summary>
    /// return string added 0 to front with given vale. 9-->09
    /// depended on given parameter (10/100/1000)
    /// </summary>
    public class IntToManagedStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int param = System.Convert.ToInt32((string)parameter);
            int integer = (int)value;


            switch (param)
            {
                case 10: return (integer < 10) ? "0" + integer : integer + "";

                case 100: return integer < 100 ? integer < 10 ? "00" + integer : "0" + integer : integer + "";

                case 1000: return integer < 1000 ? integer < 100 ? integer < 10 ? "000" + integer : "00" + integer : "0" + integer : integer + "";

                default: return integer + "";
                
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToInt32((string)value);
        }
    }
}
