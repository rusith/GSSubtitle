using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GSSubtitle.Tools.Extensions
{
    public static class RegexMatchExtension
    {
        public static int EndIndex(this Match match)
        {
            return match.Index + match.Length-1;
        }
    }
}
