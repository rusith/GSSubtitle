using GSSubtitle.Controllers.UnReDoPattern;
using GSSubtitle.Models;
using GSSubtitle.Models.SubtitleItems;
using GSSubtitle.Models.SubtitleItems.LineItems;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GSSubtitle.Tools.SubtitleTools
{
    public class SubtitleTools
    {
        public static SubtitleFormat DetectSubtitleFormat(string filepath, bool verify)
        {
            var extension = System.IO.Path.GetExtension(filepath).Trim('.');

            SubtitleFormat formatbyextension;
            //detect format using extension
            switch (extension.ToLower())
            {
                case "srt":
                    formatbyextension = SrtTools.VerifySrtFile(filepath) ? SubtitleFormat.SRT : SubtitleFormat.UNKNOWN;
                    break;
                default:
                    formatbyextension = SubtitleFormat.UNKNOWN;
                    break;
            }

            //if cannot detect using extension try verify all types and set correct one
            if (formatbyextension == SubtitleFormat.UNKNOWN)
                formatbyextension = SrtTools.VerifySrtFile(filepath) ? SubtitleFormat.SRT : SubtitleFormat.UNKNOWN;


            return formatbyextension;
        }



        private static void DR(string text)
        {
            Debug.WriteLine(text);
        }

        public static Line[] SplitLine(LineList lines,int index)
        {
            Line line = lines[index];
            var nextline = new Line();
            if (lines.Count - 1 < index + 1)
            {
                nextline.StartTime.FromMilliSeconds(line.EndTime.ToMilliSeconds()+ 2000);
            }
                
            if (line == null || nextline == null) return null;
            double duration = line.EndTime.ToMilliSeconds() - line.StartTime.ToMilliSeconds();
            duration = duration / 2;
            var line1 = new Line();
            LineList.CopyTwoLineTimes(line.EndTime,line1.StartTime);
            line.Duration = (duration/1000);
            var line2 = new Line();
            LineList.CopyTwoLineTimes(line1.EndTime, line2.StartTime);
            line2.Duration = duration / 1000;
            var linetext = StringTools.SplitText(line.Context);
            line1.Context = linetext[0];
            line1.ConstantContext = linetext[0];
            line2.Context = linetext[1];
            line2.ConstantContext = linetext[1];
            return new Line[] { line1, line2 };
        }

    }
}
