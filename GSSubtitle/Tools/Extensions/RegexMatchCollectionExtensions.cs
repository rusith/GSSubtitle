using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GSSubtitle.Tools.Extensions
{
    public static class RegexMatchCollectionExtensions
    {
        public static Match Last(this MatchCollection collection)
        {
            return collection[collection.Count - 1];
        }

        
    }
}
