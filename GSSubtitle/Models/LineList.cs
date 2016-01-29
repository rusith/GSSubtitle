using GSSubtitle.Controllers.UnReDoPattern;
using GSSubtitle.Models.SubtitleItems;
using GSSubtitle.Models.SubtitleItems.LineItems;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace GSSubtitle.Models
{
    public class LineList :List<Line>
    {
        public bool SkipUndo = true;

        public void Add(Line line,bool UndoAble)
        {
            using (UndoAble?URD.C(this)?new ListChangeAddElement(this,line,string.Format("New Line Add To Line Number {0} ",line.LineNumber)):null:null)
               base.Add(line);
        }

        public void RemoveAt(int index, bool UndoAble)
        {
            using (UndoAble ? URD.C(this)? new ListChangeRemoveElement(this, this[index], index, string.Format("Line Number {0} Remove", this[index].LineNumber + "")) :null: null)
                base.RemoveAt(index);
            
        }

        public void Insert(int index, Line line, bool UndoAble)
        {
            using (UndoAble?URD.C(this)?new ListChangeAddElement(this, line, string.Format("Add New Line-> Number :{0}",line.LineNumber)):null:null)
                base.Insert(index, line);
            
        }

        public void InsertRange(int index, IEnumerable<Line> items,bool UndoAble)
        {
            using (UndoAble ? URD.C(this)? new ListChangeAddElementRange(this, items, index, string.Format("{0} Items Added To The List", items.Count())) : null:null)
                base.InsertRange(index, items);
            
        }

        public void Remove(Line line, bool UndoAble)
        {
            using (UndoAble?URD.C(this)?new ListChangeRemoveElement(this, line, this.IndexOf(line), "Element Removed"):null:null)
                base.Remove(line);
        }

        public void FixLineNumberSequence(int startindex,bool UndoAble)
        {
            if (Count < 1||startindex>Count-2) return;
            if (startindex < 0) startindex = 0;
            int linenumber = this[startindex].LineNumber;
            for (int i = startindex + 1; i <Count; i++,linenumber++)
                this[i].ChangeLineNumber(linenumber, UndoAble);
            
        }


        public  void AddEmptyLine(int centerlocation, bool beforeorafter, bool UndoAble)
        {
            using (UndoAble ? URD.C(this) ? new ChangeCollection("New Line Added TO  " + (beforeorafter ? "Before" : "After") + "Selected Line"):null:null)
            {
                if (beforeorafter)
                {
                    bool havebeforeline = centerlocation != 0;
                    Line line = new Line { Context = "", ConstantContext = "", LineNumber = this[centerlocation].LineNumber };

                    if (havebeforeline)
                    {
                        CopyTwoLineTimes(this[centerlocation - 1].EndTime, line.StartTime);
                        double intarval = GetLineInterval(centerlocation - 1, centerlocation);
                        line.Duration = (intarval >= 2000 ? 2000 : intarval > 0 ? intarval : -1) / 1000;

                    }
                    else
                    {
                        line.StartTime = new LineTime { MilliSeconds = 0, Seconds = 0, Minutes = 0, Hours = 0 };
                        double currwntlinemilliseconds = this[centerlocation].StartTime.ToMilliSeconds();
                        line.Duration = (currwntlinemilliseconds < 1 ? 250 : currwntlinemilliseconds) / 1000;

                    }
                    Insert(centerlocation, line, UndoAble);
                    ShiftLineNumbers(centerlocation + 1, 1, true, UndoAble);
                }
                else
                {

                    bool havenextline = centerlocation + 1 <= this.Count;

                    Line line = new Line { ConstantContext = "", Context = "", LineNumber = this[centerlocation].LineNumber + 1 };
                    if (havenextline)
                    {
                        CopyTwoLineTimes(this[centerlocation + 1].EndTime, line.StartTime);
                        double intarval = this.GetLineInterval(centerlocation, centerlocation + 1);
                        line.Duration = (intarval >= 2000 ? 2000 : intarval > 0 ? intarval : -1) / 1000;
                        Insert(centerlocation + 2, line, UndoAble);
                        ShiftLineNumbers(centerlocation + 2, 1, true, UndoAble);

                    }
                    else
                    {

                        CopyTwoLineTimes(this[centerlocation].EndTime, line.StartTime);
                        line.Duration = 250;
                        Add(line, UndoAble);

                    }
                }
            }
 
        }

        public static LineTime CopyTwoLineTimes(LineTime copyFrom, LineTime copyTo)
        {
            if (copyFrom == null) copyFrom = new LineTime();
            if (copyTo == null) copyTo = new LineTime();
            copyTo.Hours = copyFrom.Hours;
            copyTo.Minutes = copyFrom.Minutes;
            copyTo.Seconds = copyFrom.Seconds;
            copyTo.MilliSeconds = copyFrom.MilliSeconds;
            return copyTo;
        }

        public  void ShiftLineNumbers(int from, int by, bool upordown,  bool UndoAble)
        {
            using (UndoAble ? new ChangeCollection("Line Numbers Shift By " + by+" to"+(upordown?"up":"down")) : null)
            {
                if (from > this.Count - 1 || from < 0) return;

                if (upordown)
                    for (int i = from; i < this.Count; i++) this[i].ChangeLineNumber(this[i].LineNumber + by, UndoAble);
                else for (int i = from; i >= 0; i--) this[i].ChangeLineNumber(this[i].LineNumber + by, UndoAble);
            }

        }

        public  double GetLineInterval(int line1, int line2 )
        {
            double firstlinemilliseconds = this[line1].EndTime.ToMilliSeconds();
            double lastlinemilliseconds = this[line2].StartTime.ToMilliSeconds();
            return lastlinemilliseconds - firstlinemilliseconds;
        }

    }
}
