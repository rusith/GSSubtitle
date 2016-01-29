using GSSubtitle.Models.SubtitleItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSSubtitle.Models
{
    public enum SubtitleFormat
    {
        SRT,UNKNOWN
    }
    public class Subtitle :BindingBase
    {
        private LineList _Lines = new LineList();
        //private string _Context;
        private SubtitleFormat _Format = new SubtitleFormat();

        #region Properties
        public LineList Lines
        {
            get
            {
                return _Lines;
            }

            set
            {
                _Lines = value;
                OPC("Lines");
            }
        }

        //public string Context
        //{
        //    get
        //    {
        //        return _Context;
        //    }

        //    set
        //    {
        //        _Context = value;
        //        OPC("Lines");
        //    }
        //}

        public SubtitleFormat Format
        {
            get
            {
                return _Format;
            }

            set
            {
                _Format = value;
                OPC("Lines");
            }
        }
        #endregion



    }
}
