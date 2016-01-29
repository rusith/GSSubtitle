using GSSubtitle.Models.SubtitleItems.LineItems;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace GSSubtitle.Tools.SubtitleTools.ValueConverters
{
    public class LineTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return value == null ? null : ((LineTime)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            var match = Regex.Match((string)value, @"\d{1,2}:\d{1,2}:\d{1,2}[:,]\d{1,3}");
            if (match.Success)
            {
                var digitmatchings = Regex.Matches(match.Value, @"\d{1,3}");
                return digitmatchings.Count > 3 ?
                    new LineTime
                    {
                        Hours = System.Convert.ToInt32(digitmatchings[0].Value),
                        Minutes = System.Convert.ToInt32(digitmatchings[1].Value),
                        Seconds = System.Convert.ToInt32(digitmatchings[2].Value),
                        MilliSeconds = System.Convert.ToInt32(digitmatchings[3].Value)
                    } : null;
            }
            else return null;
        }
    }
}
