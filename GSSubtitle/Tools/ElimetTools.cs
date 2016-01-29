using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace GSSubtitle.Tools
{
    public class ElimetTools
    {
        public static MediaState GetMediaStateOfMediaEliment(MediaElement mediaeliment)
        {
            FieldInfo hlp = typeof(MediaElement).GetField("_helper", BindingFlags.NonPublic | BindingFlags.Instance);
            object helperobject = hlp.GetValue(mediaeliment);
            FieldInfo statefield = helperobject.GetType().GetField("_currentState", BindingFlags.NonPublic | BindingFlags.Instance);
            MediaState state = (MediaState)statefield.GetValue(helperobject);
            return state;
        }

        public static BitmapImage BitmapImageFromString(string source)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(source, UriKind.Relative);
            image.EndInit();
            return image;
        }
    }
}
