using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GSSubtitle.Tools.Extensions
{
    public static class ColorExtensions
    {
        public static string ToHex(this System.Drawing.Color color)
        {
            return string.Format("#{0:x2}{1:x2}{2:x2}", color.R, color.G, color.B);
        }
    }
}
