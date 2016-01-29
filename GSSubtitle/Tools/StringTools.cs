using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GSSubtitle.Tools
{
    class StringTools
    {
 
        public static string SubstrinString(string stringtosubstring, int startindex, int endindex,bool fixlenght)
        {
            if (stringtosubstring.Length < endindex)
                if (fixlenght) endindex = stringtosubstring.Length ;
                else throw new ArgumentException("string is larger than end index");

            if (startindex > endindex)
                if (fixlenght) startindex = endindex - 1;
                else throw new ArgumentException("start index is larger than end index");
            if (startindex < 0)
                if (fixlenght) startindex = 0;


            return stringtosubstring.Substring(startindex, endindex - startindex);

        }

        public static string ColorToHex(Color c)
        {
            return string.Format("#{0:x2}{1:x2}{2:x2}", c.R, c.G, c.B);
        }

        public static string ToSuperscript(string text)
        {
            var sb = new StringBuilder();
            var superscript = new List<char>{
                                              '⁰',
                                              '¹',
                                              '²',
                                              '³',
                                              '⁴',
                                              '⁵',
                                              '⁶',
                                              '⁷',
                                              '⁸',
                                              '⁹',
                                              '⁺',
                                              '⁻',
                                              '⁼',
                                              '⁽',
                                              '⁾',
                                              'ᵃ',
                                              'ᵇ',
                                              'ᶜ',
                                              'ᵈ',
                                              'ᵉ',
                                              'ᶠ',
                                              'ᵍ',
                                              'ʰ',
                                              'ⁱ',
                                              'ʲ',
                                              'ᵏ',
                                              'ˡ',
                                              'ᵐ',
                                              'ⁿ',
                                              'ᵒ',
                                              'ᵖ',
                                              'ʳ',
                                              'ˢ',
                                              'ᵗ',
                                              'ᵘ',
                                              'ᵛ',
                                              'ʷ',
                                              'ˣ',
                                              'ʸ',
                                              'ᶻ',
                                              'ᴬ',
                                              'ᴮ',
                                              'ᴰ',
                                              'ᴱ',
                                              'ᴳ',
                                              'ᴴ',
                                              'ᴵ',
                                              'ᴶ',
                                              'ᴷ',
                                              'ᴸ',
                                              'ᴹ',
                                              'ᴺ',
                                              'ᴼ',
                                              'ᴾ',
                                              'ᴿ',
                                              'ᵀ',
                                              'ᵁ',
                                              'ᵂ'
                                            };
            var normal = new List<char>{
                                         '0', // "⁰"
                                         '1', // "¹"
                                         '2', // "²"
                                         '3', // "³"
                                         '4', // "⁴"
                                         '5', // "⁵"
                                         '6', // "⁶"
                                         '7', // "⁷"
                                         '8', // "⁸"
                                         '9', // "⁹"
                                         '+', // "⁺"
                                         '-', // "⁻"
                                         '=', // "⁼"
                                         '(', // "⁽"
                                         ')', // "⁾"
                                         'a', // "ᵃ"
                                         'b', // "ᵇ"
                                         'c', // "ᶜ"
                                         'd', // "ᵈ"
                                         'e', // "ᵉ"
                                         'f', // "ᶠ"
                                         'g', // "ᵍ"
                                         'h', // "ʰ"
                                         'i', // "ⁱ"
                                         'j', // "ʲ"
                                         'k', // "ᵏ"
                                         'l', // "ˡ"
                                         'm', // "ᵐ"
                                         'n', // "ⁿ"
                                         'o', // "ᵒ"
                                         'p', // "ᵖ"
                                         'r', // "ʳ"
                                         's', // "ˢ"
                                         't', // "ᵗ"
                                         'u', // "ᵘ"
                                         'v', // "ᵛ"
                                         'w', // "ʷ"
                                         'x', // "ˣ"
                                         'y', // "ʸ"
                                         'z', // "ᶻ"
                                         'A', // "ᴬ"
                                         'B', // "ᴮ"
                                         'D', // "ᴰ"
                                         'E', // "ᴱ"
                                         'G', // "ᴳ"
                                         'H', // "ᴴ"
                                         'I', // "ᴵ"
                                         'J', // "ᴶ"
                                         'K', // "ᴷ"
                                         'L', // "ᴸ"
                                         'M', // "ᴹ"
                                         'N', // "ᴺ"
                                         'O', // "ᴼ"
                                         'P', // "ᴾ"
                                         'R', // "ᴿ"
                                         'T', // "ᵀ"
                                         'U', // "ᵁ"
                                         'W', // "ᵂ"
                                            };
            for (int i = 0; i < text.Length; i++)
            {
                char s = text[i];
                int index = normal.IndexOf(s);
                if (index >= 0)
                    sb.Append(superscript[index]);
                else
                    sb.Append(s);
            }
            return sb.ToString();
        }

        public static string ToSubscript(string text)
        {
            var sb = new StringBuilder();
            var subcript = new List<char>{
                                           '₀',
                                           '₁',
                                           '₂',
                                           '₃',
                                           '₄',
                                           '₅',
                                           '₆',
                                           '₇',
                                           '₈',
                                           '₉',
                                           '₊',
                                           '₋',
                                           '₌',
                                           '₍',
                                           '₎',
                                           'ₐ',
                                           'ₑ',
                                           'ᵢ',
                                           'ₒ',
                                           'ᵣ',
                                           'ᵤ',
                                           'ᵥ',
                                           'ₓ',
                                            };
            var normal = new List<char>
                             {
                               '0',  // "₀"
                               '1',  // "₁"
                               '2',  // "₂"
                               '3',  // "₃"
                               '4',  // "₄"
                               '5',  // "₅"
                               '6',  // "₆"
                               '7',  // "₇"
                               '8',  // "₈"
                               '9',  // "₉"
                               '+',  // "₊"
                               '-',  // "₋"
                               '=',  // "₌"
                               '(',  // "₍"
                               ')',  // "₎"
                               'a',  // "ₐ"
                               'e',  // "ₑ"
                               'i',  // "ᵢ"
                               'o',  // "ₒ"
                               'r',  // "ᵣ"
                               'u',  // "ᵤ"
                               'v',  // "ᵥ"
                               'x',  // "ₓ"
                             };
            for (int i = 0; i < text.Length; i++)
            {
                char s = text[i];
                int index = normal.IndexOf(s);
                if (index >= 0)
                    sb.Append(subcript[index]);
                else
                    sb.Append(s);
            }
            return sb.ToString();
        }


        public static string[] SplitText(string StringToSplit)
        {
            //if (string.IsNullOrWhiteSpace(StringToSplit)) return new string[] { "", "" };
            //if (StringToSplit.Length < 2) return new string[] { string.Copy(StringToSplit), "" };
            //string[] splitted = new string[2];
            //var LineBrMatches = Regex.Matches(StringToSplit,@"<\s*b\s*r\s*>",RegexOptions.IgnoreCase);
            //if (LineBrMatches.Count > 0)
            //{
            //    if (LineBrMatches.Count == 1)
            //    {

            //        splitted[0] = SubstrinString(StringToSplit, 0, LineBrMatches[0].Index, true);
            //        splitted[1] = SubstrinString(StringToSplit, LineBrMatches[0].Index + LineBrMatches[0].Length, StringToSplit.Length, true);
            //        return splitted;
            //    }

            //}
            //var tags = new List<MatchENH>();

            //var maches=Regex.Matches(StringToSplit, @"<.+?>");

            //foreach (Match tag in maches)
            //{

            //    StringToSplit = StringToSplit.ReplaceRangeWithChar('\uFFFF', tag.Index, tag.Index + tag.Length);
            //    tags.Add(new MatchENH
            //    {
            //        match = tag,
            //        value = StringToSplit.SubstrinString(0, tag.Index, true)

            //    });
            //}
            //StringToSplit = StringToSplit.Replace("\uFFFF", "");

            //splitted[0] = StringToSplit.SubstrinString(0, StringToSplit.Length / 2, true);
            //splitted[1] = StringToSplit.SubstrinString( StringToSplit.Length / 2, StringToSplit.Length, true);

            //if (!(splitted[0].Last() == ' ' || splitted[1].Last() == ' '))
            //{
            //    int string2nextspace = 0, string1lastspace = 0;
            //    var regex0 = Regex.Matches(splitted[0], " ");
            //    var regex1 = Regex.Matches(splitted[1], " ");
            //    if (regex0.Count > 0)
            //    {
            //        string1lastspace = StringToSplit.Length-regex0[regex0.Count - 1].Index;
            //    }
            //    else
            //    {
            //        string1lastspace = -1;
            //    }
            //    if (regex1.Count > 0)
            //    {
            //        string2nextspace = regex1[0].Index;
            //    }
            //    else
            //    {
            //        string2nextspace = -1;
            //    }
            //    if (string1lastspace == -1 && string2nextspace == -1)
            //    { }
            //    else if (string1lastspace > string2nextspace)
            //    {
            //        splitted[0] = splitted[1].SubstrinString(0, string2nextspace, true) + splitted[0];
            //        splitted[1] = splitted[1].SubstrinString(string2nextspace, splitted[1].Length, true);
            //    }
            //    else if (string1lastspace < string2nextspace)
            //    {
            //        splitted[1] = splitted[0].SubstrinString(string1lastspace, splitted[1].Length, true);
            //        splitted[0] = splitted[0].SubstrinString(0, string1lastspace, true);
            //    }
            //    else { }


            //}

            //foreach (MatchENH item in tags)
            //{
            //    if (item.match.Index == 0) splitted[0].ReplaceRange(item.value, item.match.Index);
            //    else
            //    {
            //        if (item.match.Index > splitted[0].Length - 1)
            //        {
            //            splitted[1] = splitted[1].ReplaceRange(item.match.Value, item.match.Index - splitted[0].Length);
            //        }
            //        else
            //        {

            //        }
            //    }
            //}

            ////int str1added = 0, str2added = 0;
            ////foreach (Match tag in tags)
            ////{
            ////    if (tag.Index  > splitted[0].Length - 1-str1added)
            ////    {
            ////        splitted[1] = splitted[1].Insert(tag.Index - splitted[0].Length-str1added, tag.Value);
            ////        str2added = str2added + tag.Length;
            ////    }
            ////    else
            ////    {
            ////        splitted[0] = splitted[0].Insert(tag.Index-str2added, tag.Value);
            ////        str1added = str1added + tag.Length;
            ////    }
            ////}
            ////foreach (Match tag in tags)
            ////{
            ////    if (tag.Index > splitted[0].Length - 1)
            ////    {
            ////        splitted[1] = splitted[1].Replace(,);
            ////    }
            ////}




            //return splitted;



            var splitted = new string[2];
            if (StringToSplit.Length < 2)
            {
                splitted[0] = StringToSplit;
                splitted[1] = "";
                return splitted;
            }
            //var LineBrMatches = Regex.Matches(StringToSplit, @"<\s*/\s*b\s*r\s*>", RegexOptions.IgnoreCase);
            //if (LineBrMatches.Count > 0)
            //{
            //    if (LineBrMatches.Count == 1)
            //    {

            //        splitted[0] = SubstrinString(StringToSplit, 0, LineBrMatches[0].Index, true);
            //        splitted[1] = SubstrinString(StringToSplit, LineBrMatches[0].Index + LineBrMatches[0].Length, StringToSplit.Length, true);
            //        return splitted;
            //    }

            //}
            //else
            //{

            //}
            int division = StringToSplit.Length / 2;
            splitted = new string[]
            {
                SubstrinString(StringToSplit, 0, division, true),
                SubstrinString(StringToSplit, division, StringToSplit.Length, true)
            }; //split to exactly 2 
            string str1 = splitted[0];
            string str2 = splitted[1];

            if (str2.Contains(' '))
            {
                int i = str2.IndexOf(' ');
                str1 = str1 + str2.Substring(0, i);
                str2 = SubstrinString(str2, i, str2.Length, true);
            }
            var endmatch = Regex.Matches(str2, @".{0,}\s{0,}/\s{0,}>");
            var endnormalmatch = Regex.Matches(str2, @".{0,}>");
            bool Isstring2hasincompletetag = true;
            if (endnormalmatch.Count > 0)
            {
                if (!endnormalmatch[0].Value.Contains("<")) Isstring2hasincompletetag = true;
                else Isstring2hasincompletetag = false;
            }
            else Isstring2hasincompletetag = false;

            if (Isstring2hasincompletetag)
            {
                str1 = str1 + SubstrinString(str2, endnormalmatch[0].Index, endnormalmatch[0].Index + endnormalmatch[0].Length, true);
                str2 = SubstrinString(str2, endnormalmatch[0].Index + endnormalmatch[0].Length, str2.Length - 1, true);
            }
            str1 = HtmlTools.FixTags(str1);
            str2 = HtmlTools.CopyTags(str1, str2, false);
            return new string[] { str1, str2 };
        }


        public static string CombineStringArray(string[] array, char joinchar)
        {
            if (array == null) return "";
            if (array.Length == 1) return array[0];
            string returnstring = array[0];
            for (int i = 1; i < array.Length; i++) returnstring = returnstring + joinchar + array[i];
            return returnstring;
        }


        public static string[] SUbBlaBlaArray(string[] array,int starting,int ending)
        {
            if (array == null || array.Length < 1) return new string[] { ""};
            List<string> end = new List<string>();
            if (starting < 0) starting = 0;
            if (ending > array.Length - 1) ending = array.Length - 1;
            for (int i = starting; i < ending; i++) end.Add(array[i]);
            return end.ToArray();
        }
    }

    public class MatchENH
    {
        public Match match;
        public string value;
        public MatchENH()
        {
            match = null;
            value = "";
        }
    }

}
