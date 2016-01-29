using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSSubtitle.Tools
{
    static class LevenshteinDistance
    {
        public static int Compute(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0) return m;
            if (m == 0) return n;

            for (int i = 0; i <= n; d[i, 0] = i++) { }
            for (int j = 0; j <= m; d[0, j] = j++) { }

            for (int i = 1; i <= n; i++)
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            return d[n, m];

        }

        public static List<string> SortListByMatching(string matchto, List<string> list)
        {
            if (list.Count < 1) return null;
            if (string.IsNullOrWhiteSpace(matchto)) return null;
            if (list.Count == 1) return list;

            List<MatchSec> matchessequence = new List<MatchSec>();
            for (int i = 0; i < list.Count; i++)
            {

                matchessequence.Add(new MatchSec { index= Compute(matchto, list[i]) ,value=list[i]});
            }

            matchessequence= matchessequence.OrderBy(o => o.index).ToList() ;
            matchessequence.Reverse();
            var returnlist = new List<string>();
            foreach (var item in matchessequence)
            {
                returnlist.Add(item.value);
            }
            return returnlist;
        }



    }


    public class MatchSec
    {
        public int index;
        public string value;
    }
}
