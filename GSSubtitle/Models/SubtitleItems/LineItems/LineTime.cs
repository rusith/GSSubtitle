using System;
using System.Text;
using System.Text.RegularExpressions;

namespace GSSubtitle.Models.SubtitleItems.LineItems
{
    public class LineTime:BindingBase
    {
        private int _Hours;
        private int _Minutes;
        private int _Seconds;
        private int _MilliSeconds;

        #region Properties

        public int Hours
        {
            get
            {
                return _Hours;
            }

            set
            {
                _Hours = value;
                OPC("Hours");
            }
        }

        public int Minutes
        {
            get
            {
                return _Minutes;
            }

            set
            {
                while (value > 59)
                {
                    value -= 60;
                    Hours++;
                }
                while (value < 0)
                {
                    value += 60;
                    Hours--;
                }
                _Minutes = value;
                OPC("Minutes");
            }
        }

        public int Seconds
        {
            get
            {
                return _Seconds;
            }

            set
            {
                while (value > 59)
                {
                    Minutes++;
                    value -= 60;
                }
                while (value < 0)
                {
                    value += 60;
                    Minutes--;
                }
                _Seconds = value;
                OPC("Seconds");
            }
        }

        public int MilliSeconds
        {
            get
            {
                return _MilliSeconds;
            }

            set
            {
                while (value > 999)
                {
                    Seconds++;
                    value-=1000;
                }
                while (value < 0)
                {
                    value += 1000;
                    Seconds--;
                }

                _MilliSeconds = value;
                OPC("MilliSeconds");
            }
        }
        #endregion

        public LineTime() { }

        public LineTime(TimeSpan timespan)
        {
            FromTimeSpan(timespan);
        }

        public LineTime(double milliseconds)
        {
            FromMilliSeconds(milliseconds);
        }

        public LineTime(string text)
        {
            FromString(text);
        }


        public void FromTimeSpan(TimeSpan timespan)
        {
            MilliSeconds = timespan.Milliseconds;
            Seconds = timespan.Seconds;
            Minutes = timespan.Minutes;
            Hours = timespan.Hours;
        }

        public void FromMilliSeconds(double milliseconds)
        {
            FromTimeSpan(TimeSpan.FromMilliseconds(milliseconds));
        }

        public void FromString(string text)
        {
            var match = Regex.Match(text, @"\d{1,2}:\d{1,2}:\d{1,2}[:,]\d{1,3}");
            if (match.Success == true)

            {
                var digitmatchings = Regex.Matches(match.Value, @"\d{1,3}");
                if (digitmatchings.Count > 3)
                {
                    Hours = Convert.ToInt32(digitmatchings[0].Value);
                    Minutes = Convert.ToInt32(digitmatchings[1].Value);
                    Seconds = Convert.ToInt32(digitmatchings[2].Value);
                    MilliSeconds = Convert.ToInt32(digitmatchings[3].Value);
                }



            }
        }

        public static LineTime NewFromString(string text)
        {
            var match = Regex.Match(text, @"\d{1,2}:\d{1,2}:\d{1,2}[:,]\d{1,3}");
            if (match.Success == true)

            {
                var digitmatchings = Regex.Matches(match.Value, @"\d{1,3}");
                if (digitmatchings.Count > 3)
                {
                    return new LineTime
                    {
                        Hours = Convert.ToInt32(digitmatchings[0].Value),
                        Minutes = Convert.ToInt32(digitmatchings[1].Value),
                        Seconds = Convert.ToInt32(digitmatchings[2].Value),
                        MilliSeconds = Convert.ToInt32(digitmatchings[3].Value)
                    };

                }
                else return null;



            }
            else return null;
        }

        public TimeSpan ToTimeSpan()
        {
            return new TimeSpan(0, Hours, Minutes, Seconds, MilliSeconds);
        }

        public override string ToString()
        {
            var stringbuilder = new StringBuilder("");
            if (Hours < 10)
                stringbuilder.Append("0" + Hours + ":");
            else
                stringbuilder.Append(Hours + ":");

            if (Minutes < 10)
                stringbuilder.Append("0" + Minutes + ":");
            else
                stringbuilder.Append(Minutes + ":");

            if (Seconds < 10)
                stringbuilder.Append("0" +Seconds + ":");
            else
                stringbuilder.Append(Seconds + ",");

            if (MilliSeconds < 100)
                if (MilliSeconds < 10)
                    stringbuilder.Append("00" + MilliSeconds);
                else
                    stringbuilder.Append("0" + MilliSeconds);
            else
                stringbuilder.Append(MilliSeconds);

            return stringbuilder.ToString();
        }

        public double ToMilliSeconds()
        {
            return new TimeSpan(0, Hours, Minutes, Seconds, MilliSeconds).TotalMilliseconds;
        }

        public LineTime AddSomeMilliSeconds( int milliseconds)
        {
            MilliSeconds = MilliSeconds + milliseconds;
            return this;
        }


    }
}
