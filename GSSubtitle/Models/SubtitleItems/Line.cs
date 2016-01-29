using GSSubtitle.Controllers.UnReDoPattern;
using GSSubtitle.Models.SubtitleItems.LineItems;
using GSSubtitle.Tools.SubtitleTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSSubtitle.Models.SubtitleItems
{
    public class Line:BindingBase
    {
        private string _Context;
        private  string _ConstantContext;
        private LineTime _StartTime=new LineTime();
        private LineTime _EndTime =new LineTime();
        private int _LineNumber;




        #region Properties

        public string Context
        {
            get
            {
                return _Context;
            }

            set
            {
                _Context = value;
                OPC("Context");

            }
        }


        public LineTime StartTime
        {
            get
            {
                return _StartTime;
            }

            set
            {
                if (value == null)
                    return;
                if (value.ToMilliSeconds() > EndTime.ToMilliSeconds())
                    return;
                _StartTime = value;
                OPC("StartTime");
                OPC("Duration");
            }
        }

        public LineTime EndTime
        {
            get
            {
                return _EndTime;
            }

            set
            {
                if (value == null)
                    return;
                _EndTime = value;
                OPC("EndTime");
                OPC("Duration");
            }
        }

        public int LineNumber
        {
            get
            {
                return _LineNumber;
            }

            set
            {
                _LineNumber = value;
                OPC("LineNumber");
            }
        }

        public string ConstantContext
        {
            get
            {
                return _ConstantContext;
            }

            set
            {
                _ConstantContext = value;
                OPC("ConstantContext");
            }
        }

        public double Duration
        {
            get
            {
                return (EndTime.ToMilliSeconds() - StartTime.ToMilliSeconds()) / 1000;
            }
                
            
            set
            {
                if (value < 0)
                    return;
                ChangeDuration(value, false);
                OPC("StartTime");
                OPC("Duration");
                OPC("EndTime");
            }
        }
        #endregion

        public void ClearContext(bool undoAble)
        {
            using (undoAble ? URD.C(this, "Context")? new PropertyChange(this, "Context", string.Format("Line Number {0} Text Clear", LineNumber)):null:null)
                Context = "";
            
        }

        public void ClearConstantContext(bool UndoAble)
        {
            using (UndoAble ? URD.C(this, "ConstantContext") ? new PropertyChange(this, "ConstantContext", string.Format("Line Number {0} Original Text Clear", LineNumber)):null:null)
                ConstantContext = "";
            
        }

        public void SetContext(string str,bool UndoAble,string description=null )
        {
            using (UndoAble ? URD.C(this,"Context")? new PropertyChange(this, "Context", description==null?string.Format("Context Of Line Number {0}  {1} to {2}", LineNumber, _Context, str):description) : null :null)
                Context = str;
                  
        }
        public void SetConstantContext(string str, bool UndoAble)
        {
            using (UndoAble ? URD.C(this, "ConstantContext")? new PropertyChange(this, "ConstantContext", string.Format("Context Of Line Number {0}  {1} to {2}", LineNumber, _ConstantContext, str)) :null: null)
                ConstantContext = str;
            

        }

        public void ChangeLineNumber(int NewLineNumber, bool UndoAble)
        {
            using (UndoAble ? URD.C(this, "LineNumber") ?new PropertyChange(this, "LineNumber", string.Format("Line Number Update {0} TO {1}", _LineNumber, NewLineNumber)) : null:null)
                LineNumber = NewLineNumber;  
        }

        public void ChangeDuration(Double duration,bool UndoAble)
        {
            if (duration != Duration)
            {
                using (UndoAble ? URD.C(this, "Duration") ? new PropertyChange(this, "Duration", string.Format("Line Number {0} Duration Changed", Duration)) : null : null)
                    EndTime.FromMilliSeconds(StartTime.ToMilliSeconds() + TimeSpan.FromSeconds(duration).TotalMilliseconds);
            }
   
                
        }
    }
}
