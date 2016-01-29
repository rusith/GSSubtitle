using GSSubtitle.Models.SubtitleItems;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GSSubtitle.Tools.SubtitleTools.ValueConverters
{
    public class BindingBrakedLinesConverter : IMultiValueConverter
    {
        bool check;
        Line line=new Line();
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == null)
                return "";
            check = (bool)values[1];
            line = (Line)values[0];
            return ((Line)values[0]).Context.Replace( "</br>", Environment.NewLine);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            string returnstring = "";

            if(value!=null)
            {
                returnstring = (string)value;
                if (check == true)
                    returnstring = returnstring.Replace(Environment.NewLine, "</br>");
                else
                    returnstring = returnstring.Replace(Environment.NewLine, "");
            }
            line.Context = returnstring;
            return new object[] { line,check };
        }
    }
}
