using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GSSubtitle.Controllers.UnReDoPattern
{
    public interface IUndoAble
    {
        void Undo();
        void Redo();

    }
}
