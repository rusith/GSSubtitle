using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GSSubtitle.Tools
{
    public static class StringExtensions
    {
        /// <summary>
        /// exactly find a text starts with some text using regex
        /// </summary>
        /// <param name="texttocheck">automatically get current text .becouse this is a extension</param>
        /// <param name="text">text for use to check </param>
        /// <param name="dontSkipLeadingSpaces">
        ///                          if true leading spaces not ignored 
        ///                          else leading spaces will ignored</param>
        /// <param name="RegexOPtion">Regex Option For The Check</param>
        /// <returns></returns>
        public static bool StartsWith(this string texttocheck, string text,bool dontSkipLeadingSpaces,RegexOptions RegexOPtion)
        {
            var match = Regex.Match(texttocheck,  dontSkipLeadingSpaces?""+text: @"\s{0,}" + text, RegexOPtion);
            return (match.Success && match.Index == 0);
        }
        public static bool LineStartsWithHtmlTag(this string text, bool threeLengthTag, bool includeFont = false)
        {
            if (text == null || (!threeLengthTag && !includeFont))
                return false;
            return StartsWithHtmlTag(text, threeLengthTag, includeFont);
        }

        public static bool LineEndsWithHtmlTag(this string text, bool threeLengthTag, bool includeFont = false)
        {
            if (text == null)
                return false;

            var len = text.Length;
            if (len < 6 || text[len - 1] != '>')
                return false;

            // </font> </i>
            if (threeLengthTag && len > 3 && text[len - 4] == '<' && text[len - 3] == '/')
                return true;
            if (includeFont && len > 8 && text[len - 7] == '<' && text[len - 6] == '/')
                return true;
            return false;
        }

        public static bool LineBreakStartsWithHtmlTag(this string text, bool threeLengthTag, bool includeFont = false)
        {
            if (text == null || (!threeLengthTag && !includeFont))
                return false;
            var newLineIdx = text.IndexOf(Environment.NewLine, StringComparison.Ordinal);
            if (newLineIdx < 0 || text.Length < newLineIdx + 5)
                return false;
            text = text.Substring(newLineIdx + 2);
            return StartsWithHtmlTag(text, threeLengthTag, includeFont);
        }

        private static bool StartsWithHtmlTag(string text, bool threeLengthTag, bool includeFont)
        {
            if (threeLengthTag && text.Length >= 3 && text[0] == '<' && text[2] == '>' && (text[1] == 'i' || text[1] == 'I' || text[1] == 'u' || text[1] == 'U' || text[1] == 'b' || text[1] == 'B'))
                return true;
            if (includeFont && text.Length > 5 && text.StartsWith("<font", StringComparison.OrdinalIgnoreCase))
                return text.IndexOf('>', 5) >= 5; // <font> or <font color="#000000">
            return false;
        }

        public static bool StartsWith(this string s, char c)
        {
            return s.Length > 0 && s[0] == c;
        }

        public static bool StartsWith(this StringBuilder sb, char c)
        {
            return sb.Length > 0 && sb[0] == c;
        }

        public static bool EndsWith(this string s, char c)
        {
            return s.Length > 0 && s[s.Length - 1] == c;
        }

        public static bool EndsWith(this StringBuilder sb, char c)
        {
            return sb.Length > 0 && sb[sb.Length - 1] == c;
        }

        public static bool Contains(this string source, char value)
        {
            return source.IndexOf(value) >= 0;
        }

        public static bool Contains(this string source, char[] value)
        {
            return source.IndexOfAny(value) >= 0;
        }

        public static bool Contains(this string source, string value, StringComparison comparisonType)
        {
            return source.IndexOf(value, comparisonType) >= 0;
        }

        public static string[] SplitToLines(this string source)
        {
            return source.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        }

        // http://www.codeproject.com/Articles/43726/Optimizing-string-operations-in-C
        public static int FastIndexOf(this string source, string pattern)
        {
            if (pattern == null) throw new ArgumentNullException();
            if (pattern.Length == 0) return 0;
            if (pattern.Length == 1) return source.IndexOf(pattern[0]);
            int limit = source.Length - pattern.Length + 1;
            if (limit < 1) return -1;
            // Store the first 2 characters of "pattern"
            char c0 = pattern[0];
            char c1 = pattern[1];
            // Find the first occurrence of the first character
            int first = source.IndexOf(c0, 0, limit);
            while (first != -1)
            {
                // Check if the following character is the same like
                // the 2nd character of "pattern"
                if (source[first + 1] != c1)
                {
                    first = source.IndexOf(c0, ++first, limit - first);
                    continue;
                }
                // Check the rest of "pattern" (starting with the 3rd character)
                bool found = true;
                for (var j = 2; j < pattern.Length; j++)
                    if (source[first + j] != pattern[j])
                    {
                        found = false;
                        break;
                    }
                // If the whole word was found, return its index, otherwise try again
                if (found) return first;
                first = source.IndexOf(c0, ++first, limit - first);
            }
            return -1;
        }

        public static int IndexOfAny(this string s, string[] words, StringComparison comparionType)
        {
            if (words == null || string.IsNullOrEmpty(s))
                return -1;
            for (int i = 0; i < words.Length; i++)
            {
                var idx = s.IndexOf(words[i], comparionType);
                if (idx >= 0)
                    return idx;
            }
            return -1;
        }

        public static string FixExtraSpaces(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            while (s.Contains("  "))
                s = s.Replace("  ", " ");
            s = s.Replace(" " + Environment.NewLine, Environment.NewLine);
            return s.Replace(Environment.NewLine + " ", Environment.NewLine);
        }

        public static bool ContainsLetter(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return false;

            foreach (var c in s)
            {
                if (char.IsLetter(c))
                    return true;
            }
            return false;
        }

        public static string RemoveControlCharacters(this string s)
        {
            var sb = new StringBuilder(s.Length);
            foreach (var ch in s)
            {
                if (!Char.IsControl(ch))
                    sb.Append(ch);
            }
            return sb.ToString();
        }

        public static string RemoveControlCharactersButWhiteSpace(this string s)
        {
            var sb = new StringBuilder(s.Length);
            foreach (var ch in s)
            {
                if (!Char.IsControl(ch) || ch == '\u000d' || ch == '\u000a' || ch == '\u0009')
                    sb.Append(ch);
            }
            return sb.ToString();
        }

        public static string CapitalizeFirstLetter(this string s, CultureInfo ci = null)
        {
            var si = new StringInfo(s);
            if (ci == null)
                ci = CultureInfo.CurrentCulture;
            if (si.LengthInTextElements > 0)
                s = si.SubstringByTextElements(0, 1).ToUpper(ci);
            if (si.LengthInTextElements > 1)
                s += si.SubstringByTextElements(1);
            return s;
        }

        public static string ReplaceRangeWithChar(this string str,char cha,int startindex,int endindex)
        {
            if (startindex < 0) startindex = 0;
            if (endindex > str.Length - 1) endindex = str.Length - 1;
            if (startindex > endindex) startindex = endindex - 1;
            var chars = str.ToArray();
            for (int i = startindex; i <=endindex; i++) chars[i] = cha;
            return new string(chars);

        }

        public static string SubstrinString(this string stringtosubstring, int startindex, int endindex, bool fixlenght)
        {
            if (stringtosubstring.Length < endindex)
                if (fixlenght) endindex = stringtosubstring.Length;
                else throw new ArgumentException("string is larger than end index");

            if (startindex > endindex)
                if (fixlenght) startindex = endindex - 1;
                else throw new ArgumentException("start index is larger than end index");
            if (startindex < 0)
                if (fixlenght) startindex = 0;


            return stringtosubstring.Substring(startindex, endindex - startindex);

        }
        

        //public static string SplitTo2WithSkippingChars(this string stringtosplit, char chartosplit)
        //{
        //    for (int i = 0; i < stringtosplit.Length; i++)
        //    {

        //    }
        //}

        public static string ReplaceRange(this string stringtoreplace, string stringtoreplacewith, int index)
        {
            if (index < 0) index = 0;
            if (index > stringtoreplace.Length - 1) index = stringtoreplace.Length - 1;
            var str = stringtoreplace.ToCharArray();
            int ind = 0;
            int iii = index;
            for (int i = 0; i < stringtoreplacewith.Length; i++)
            {
                if (iii > str.Length - 1) break;

                str[iii] = stringtoreplacewith[ind];

                ind++;
                iii++;
            }

            return new string(str);

        }

    }
}
