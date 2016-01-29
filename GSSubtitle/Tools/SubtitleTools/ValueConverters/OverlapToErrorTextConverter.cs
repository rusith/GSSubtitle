using GSSubtitle.Models.SubtitleItems;
using GSSubtitle.Models.SubtitleItems.LineItems;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GSSubtitle.Tools.SubtitleTools.ValueConverters
{
    public class OverlapToErrorTextConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int selectedindex = (int)values[1];
            if (selectedindex == -1)
                return "";
            double dbl=0;
            if (!double.TryParse((string)values[3], out dbl))  return "";
            LineTime starttime = LineTime.NewFromString((string)values[2]);
            if (starttime == null) return "";
            List<Line> lines = values[0] as List<Line>;
            if (lines==null ||lines.Count < 2) return "";
            StringBuilder stringbuilder = new StringBuilder();

            double currentlinestartingmilliseconds = lines[selectedindex].StartTime.ToMilliSeconds();
            double currentlineendingmilliseconds = lines[selectedindex].EndTime.ToMilliSeconds();
            bool withpweline=false;
            bool withnextline=false;
            string perlinetext="";
            string nextlinetext="";
            if (selectedindex > 0)
            {
                double previouslinemilliseconds = lines[selectedindex - 1].EndTime.ToMilliSeconds();

                if (previouslinemilliseconds > currentlinestartingmilliseconds)
                {
                    withpweline = true;
                    perlinetext=(previouslinemilliseconds - currentlinestartingmilliseconds) / 1000+"";
                }
            }
            if (selectedindex + 1 <= lines.Count-1)
            {
                double nextlinestartingmilliseconds = lines[selectedindex + 1].StartTime.ToMilliSeconds();
                if (currentlineendingmilliseconds > nextlinestartingmilliseconds)
                {
                    withnextline = true;
                    nextlinetext=(currentlineendingmilliseconds - nextlinestartingmilliseconds)/1000+"";
                }
            }
            if (withnextline == true || withpweline == true)
            {
                stringbuilder.Append("line overlapping ");
                if (withnextline == true && withpweline == false)
                {
                    stringbuilder.Append(nextlinetext + " s with next line");
                }
                else if (withpweline = true && withnextline == false)
                {
                    stringbuilder.Append(perlinetext + " s with previous line");
                }
                else
                {
                    stringbuilder.Append(perlinetext + " s with previous line , "+ nextlinetext + " s with next line");
                }
                    
            }

            return stringbuilder.ToString();
                
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { };
        }
    }
}
