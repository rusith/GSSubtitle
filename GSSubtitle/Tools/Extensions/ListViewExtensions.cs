using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace GSSubtitle.Tools.Extensions
{
    public static class ListViewExtensions
    {
        public static bool IsSomethingSelected(this ListView LV)
        {
            return LV.SelectedItems.Count > 0 ? true : false;
        }

        public static int SelectedItemsCount(this ListView LV)
        {
            return LV.SelectedItems.Count;
        }
    }
}
