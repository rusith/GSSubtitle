using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GSSubtitle.Controllers
{
    public static class Settings
    {
        private static int _MaxUndoAndRedos=64;


        public static int MaxUndoAndRedos
        {
            get { return _MaxUndoAndRedos; }
            set{  _MaxUndoAndRedos = value > 512 ? 512 : value;  }
        }
    }
}
