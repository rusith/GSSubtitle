using GSSubtitle.Models;
using GSSubtitle.Models.SubtitleItems;
using GSSubtitle.Models.SubtitleItems.LineItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GSSubtitle.Tools.SubtitleTools
{
    public  class SrtTools
    {
        private static string _timelineRegexPattern = @"\d{1,}\s{0,}\d{2}[:]\d{2}[:]\d{2}[,]\d{3}\s{0,}.{3}\s{0,}\d{2}[:]\d{2}[:]\d{2}[,]\d{3}";
        //private static string _linetimeRegexPattern = @"\d{2}[:]\d{2}[:]\d{2}[,]\d{3}\s{0,}";
        private static string _anyDigitRegexPattern = @"\d{1,}";



        public static Subtitle ReadSrt(string filepath)
        {
            string context = System.IO.File.ReadAllText(filepath);
            var subtitleobject = new Subtitle();
            subtitleobject.Format = SubtitleFormat.SRT;
            //subtitleobject.Context = context;
            var timelinematchcollection = Regex.Matches(context,_timelineRegexPattern);

            for (int i = 0; i < timelinematchcollection.Count; i++)
            {

                Match match = timelinematchcollection[i];
                Line line = new Line();
                string timeline = match.Value;

                var lineTimes = Regex.Matches(timeline, _anyDigitRegexPattern);

                line.LineNumber = Convert.ToInt32(lineTimes[0].Value);

                line.StartTime = new LineTime();
                line.StartTime.Hours = Convert.ToInt32(lineTimes[1].Value);
                line.StartTime.Minutes = Convert.ToInt32(lineTimes[2].Value);
                line.StartTime.Seconds = Convert.ToInt32(lineTimes[3].Value);
                line.StartTime.MilliSeconds = Convert.ToInt32(lineTimes[4].Value);

                line.EndTime = new LineTime();
                line.EndTime.Hours = Convert.ToInt32(lineTimes[5].Value);
                line.EndTime.Minutes = Convert.ToInt32(lineTimes[6].Value);
                line.EndTime.Seconds = Convert.ToInt32(lineTimes[7].Value);
                line.EndTime.MilliSeconds = Convert.ToInt32(lineTimes[8].Value);

                if (i + 1 == timelinematchcollection.Count)
                    line.Context = StringTools.SubstrinString(context, match.Index + match.Length, context.Length,true).Trim(Environment.NewLine.ToArray()).Replace(Environment.NewLine,"</br>");
                    
                else
                    line.Context=StringTools.SubstrinString(context,match.Index+match.Length,timelinematchcollection[i+1].Index,true).Trim(Environment.NewLine.ToArray()).Replace(Environment.NewLine,"</br>");

                line.ConstantContext = String.Copy(line.Context);
                subtitleobject.Lines.Add(line);
                

            }
            subtitleobject.Lines.SkipUndo = false;
            return subtitleobject;

        }

        public static bool VerifySrtFile(string filepath)
        {
            if (Regex.Matches(System.IO.File.ReadAllText(filepath), _timelineRegexPattern).Count > 1)
                return true;
            return false;

        }

      

    }
}
