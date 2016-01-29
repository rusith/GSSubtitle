using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GSSubtitle.Tools.Extensions;

namespace GSSubtitle.Tools
{
    class HtmlTools
    {
        public const string TagItalic = "i";
        public const string TagBold = "b";
        public const string TagUnderline = "u";
        public const string TagParagraph = "p";
        public const string TagFont = "font";
        public const string TagCyrillicI = "\u0456";
        private static readonly Regex TagOpenRegex = new Regex(@"<\s*(?:/\s*)?(\w+)[^>]*>", RegexOptions.Compiled);
        private static readonly string[] UppercaseTags = { "<I>", "<U>", "<B>", "<FONT", "</I>", "</U>", "</B>", "</FONT>" };
        public static string RemoveHtmlTags(string s, bool alsoSsaTags = false)
        {
            if (s == null || s.Length < 3)
                return s;

            if (alsoSsaTags)
                s = RemoveSsaTags(s);

            if (!s.Contains('<'))
                return s;

            if (s.Contains("< "))
                s = FixInvalidItalicTags(s);

            return RemoveOpenCloseTags(s, TagItalic, TagBold, TagUnderline, TagParagraph, TagFont, TagCyrillicI);
        }

        public static string RemoveSsaTags(string s)
        {
            int k = s.IndexOf('{');
            while (k >= 0)
            {
                int l = s.IndexOf('}', k);
                if (l > k)
                {
                    s = s.Remove(k, l - k + 1);
                    if (s.Length > 1 && s.Length > k)
                        k = s.IndexOf('{', k);
                    else
                        break;
                }
                else
                {
                    break;
                }
            }
            return s;
        }

        public static string FixInvalidItalicTags(string text)
        {
            const string beginTag = "<i>";
            const string endTag = "</i>";

            text = text.Replace("< i >", beginTag);
            text = text.Replace("< i>", beginTag);
            text = text.Replace("<i >", beginTag);
            text = text.Replace("< I>", beginTag);
            text = text.Replace("<I >", beginTag);

            text = text.Replace("< / i >", endTag);
            text = text.Replace("< /i>", endTag);
            text = text.Replace("</ i>", endTag);
            text = text.Replace("< /i>", endTag);
            text = text.Replace("< /i >", endTag);
            text = text.Replace("</i >", endTag);
            text = text.Replace("</ i >", endTag);
            text = text.Replace("< / i>", endTag);
            text = text.Replace("< /I>", endTag);
            text = text.Replace("</ I>", endTag);
            text = text.Replace("< /I>", endTag);
            text = text.Replace("< / I >", endTag);

            text = text.Replace("</i> <i>", "_@_");
            text = text.Replace(" _@_", "_@_");
            text = text.Replace(" _@_ ", "_@_");
            text = text.Replace("_@_", " ");

            if (text.Contains(beginTag))
                text = text.Replace("<i/>", endTag);
            else
                text = text.Replace("<i/>", string.Empty);

            text = text.Replace(beginTag + beginTag, beginTag);
            text = text.Replace(endTag + endTag, endTag);

            int italicBeginTagCount = CountTagInText(text, beginTag);
            int italicEndTagCount = CountTagInText(text, endTag);
            int noOfLines = GetNumberOfLines(text);
            if (italicBeginTagCount + italicEndTagCount > 0)
            {
                if (italicBeginTagCount == 1 && italicEndTagCount == 1 && text.IndexOf(beginTag, StringComparison.Ordinal) > text.IndexOf(endTag, StringComparison.Ordinal))
                {
                    text = text.Replace(beginTag, "___________@");
                    text = text.Replace(endTag, beginTag);
                    text = text.Replace("___________@", endTag);
                }

                if (italicBeginTagCount == 2 && italicEndTagCount == 0)
                {
                    int firstIndex = text.IndexOf(beginTag, StringComparison.Ordinal);
                    int lastIndex = text.LastIndexOf(beginTag, StringComparison.Ordinal);
                    int lastIndexWithNewLine = text.LastIndexOf(Environment.NewLine + beginTag, StringComparison.Ordinal) + Environment.NewLine.Length;
                    if (noOfLines == 2 && lastIndex == lastIndexWithNewLine && firstIndex < 2)
                        text = text.Replace(Environment.NewLine, endTag + Environment.NewLine) + endTag;
                    else if (text.Length > lastIndex + endTag.Length)
                        text = text.Substring(0, lastIndex) + endTag + text.Substring(lastIndex - 1 + endTag.Length);
                    else
                        text = text.Substring(0, lastIndex) + endTag;
                }

                if (italicBeginTagCount == 1 && italicEndTagCount == 2)
                {
                    int firstIndex = text.IndexOf(endTag, StringComparison.Ordinal);
                    if (text.StartsWith("</i>-<i>-", StringComparison.Ordinal))
                        text = text.Remove(0, 5);
                    else if (text.StartsWith("</i>- <i>-", StringComparison.Ordinal))
                        text = text.Remove(0, 5);
                    else if (text.StartsWith("</i>- <i> -", StringComparison.Ordinal))
                        text = text.Remove(0, 5);
                    else if (text.StartsWith("</i>-<i> -", StringComparison.Ordinal))
                        text = text.Remove(0, 5);
                    else if (firstIndex == 0)
                        text = text.Remove(0, 4);
                    else
                        text = text.Substring(0, firstIndex) + text.Substring(firstIndex + endTag.Length);
                }

                if (italicBeginTagCount == 2 && italicEndTagCount == 1)
                {
                    var lines = SplitToLines(text);
                    if (lines.Length == 2 && lines[0].StartsWith(beginTag, StringComparison.Ordinal) && lines[0].EndsWith(endTag, StringComparison.Ordinal) &&
                        lines[1].StartsWith(beginTag, StringComparison.Ordinal))
                    {
                        text = text.TrimEnd() + endTag;
                    }
                    else
                    {
                        int lastIndex = text.LastIndexOf(beginTag, StringComparison.Ordinal);
                        if (text.Length > lastIndex + endTag.Length)
                            text = text.Substring(0, lastIndex) + text.Substring(lastIndex - 1 + endTag.Length);
                        else
                            text = text.Substring(0, lastIndex - 1) + endTag;
                    }
                    if (text.StartsWith(beginTag, StringComparison.Ordinal) && text.EndsWith(endTag, StringComparison.Ordinal) && text.Contains(endTag + Environment.NewLine + beginTag))
                    {
                        text = text.Replace(endTag + Environment.NewLine + beginTag, Environment.NewLine);
                    }
                }

                if (italicBeginTagCount == 1 && italicEndTagCount == 0)
                {
                    int lastIndexWithNewLine = text.LastIndexOf(Environment.NewLine + beginTag, StringComparison.Ordinal) + Environment.NewLine.Length;
                    int lastIndex = text.LastIndexOf(beginTag, StringComparison.Ordinal);

                    if (text.StartsWith(beginTag, StringComparison.Ordinal))
                        text += endTag;
                    else if (noOfLines == 2 && lastIndex == lastIndexWithNewLine)
                        text += endTag;
                    else
                        text = text.Replace(beginTag, string.Empty);
                }

                if (italicBeginTagCount == 0 && italicEndTagCount == 1)
                {
                    var cleanText = RemoveOpenCloseTags(text, TagItalic, TagBold, TagUnderline, TagCyrillicI);
                    bool isFixed = false;

                    // Foo.</i>
                    if (text.EndsWith(endTag, StringComparison.Ordinal) && !cleanText.StartsWith("-") && !cleanText.Contains(Environment.NewLine + "-"))
                    {
                        text = beginTag + text;
                        isFixed = true;
                    }

                    // - Foo</i> | - Foo.
                    // - Bar.    | - Foo.</i>
                    if (!isFixed && GetNumberOfLines(cleanText) == 2)
                    {
                        int newLineIndex = text.IndexOf(Environment.NewLine, StringComparison.Ordinal);
                        if (newLineIndex > 0)
                        {
                            var firstLine = text.Substring(0, newLineIndex).Trim();
                            var secondLine = text.Substring(newLineIndex + 2).Trim();
                            if (firstLine.EndsWith(endTag, StringComparison.Ordinal))
                            {
                                firstLine = beginTag + firstLine;
                                isFixed = true;
                            }
                            if (secondLine.EndsWith(endTag, StringComparison.Ordinal))
                            {
                                secondLine = beginTag + secondLine;
                                isFixed = true;
                            }
                            text = firstLine + Environment.NewLine + secondLine;
                        }
                    }
                    if (!isFixed)
                        text = text.Replace(endTag, string.Empty);
                }

                // - foo.</i>
                // - bar.</i>
                if (italicBeginTagCount == 0 && italicEndTagCount == 2 && text.Contains(endTag + Environment.NewLine) && text.EndsWith(endTag, StringComparison.Ordinal))
                {
                    text = text.Replace(endTag, string.Empty);
                    text = beginTag + text + endTag;
                }

                if (italicBeginTagCount == 0 && italicEndTagCount == 2 && text.StartsWith(endTag, StringComparison.Ordinal) && text.EndsWith(endTag, StringComparison.Ordinal))
                {
                    int firstIndex = text.IndexOf(endTag, StringComparison.Ordinal);
                    text = text.Remove(firstIndex, endTag.Length).Insert(firstIndex, beginTag);
                }

                // <i>Foo</i>
                // <i>Bar</i>
                if (italicBeginTagCount == 2 && italicEndTagCount == 2 && GetNumberOfLines(text) == 2)
                {
                    int index = text.IndexOf(Environment.NewLine, StringComparison.Ordinal);
                    if (index > 0 && text.Length > index + (beginTag.Length + endTag.Length))
                    {
                        var firstLine = text.Substring(0, index).Trim();
                        var secondLine = text.Substring(index + 2).Trim();

                        if (firstLine.Length > 10 && firstLine.StartsWith("- <i>", StringComparison.Ordinal) && firstLine.EndsWith(endTag, StringComparison.Ordinal))
                        {
                            text = "<i>- " + firstLine.Remove(0, 5) + Environment.NewLine + secondLine;
                            text = text.Replace("<i>-  ", "<i>- ");
                            index = text.IndexOf(Environment.NewLine, StringComparison.Ordinal);
                            firstLine = text.Substring(0, index).Trim();
                            secondLine = text.Substring(index + 2).Trim();
                        }
                        if (secondLine.Length > 10 && secondLine.StartsWith("- <i>", StringComparison.Ordinal) && secondLine.EndsWith(endTag, StringComparison.Ordinal))
                        {
                            text = firstLine + Environment.NewLine + "<i>- " + secondLine.Remove(0, 5);
                            text = text.Replace("<i>-  ", "<i>- ");
                            index = text.IndexOf(Environment.NewLine, StringComparison.Ordinal);
                            firstLine = text.Substring(0, index).Trim();
                            secondLine = text.Substring(index + 2).Trim();
                        }

                        if (StartsAndEndsWithTag(firstLine, beginTag, endTag) && StartsAndEndsWithTag(secondLine, beginTag, endTag))
                        {
                            text = text.Replace(beginTag, String.Empty).Replace(endTag, String.Empty).Trim();
                            text = beginTag + text + endTag;
                        }
                    }

                    //FALCONE:<i> I didn't think</i><br /><i>it was going to be you,</i>
                    var colIdx = text.IndexOf(':');
                    if (colIdx > -1 && CountTagInText(text, beginTag) + CountTagInText(text, endTag) == 4 && text.Length > colIdx + 1 && !char.IsDigit(text[colIdx + 1]))
                    {
                        var firstLine = text.Substring(0, index);
                        var secondLine = text.Substring(index).TrimStart();

                        var secIdxCol = secondLine.IndexOf(':');
                        if (secIdxCol < 0 || !IsBetweenNumbers(secondLine, secIdxCol))
                        {
                            var idx = firstLine.IndexOf(':');
                            if (idx > 1)
                            {
                                var pre = text.Substring(0, idx + 1).TrimStart();
                                text = text.Remove(0, idx + 1);
                                text = FixInvalidItalicTags(text).Trim();
                                if (text.StartsWith("<i> ", StringComparison.OrdinalIgnoreCase))
                                    text = RemoveSpaceBeforeAfterTag(text, beginTag);
                                text = pre + " " + text;
                            }
                        }
                    }
                }

                //<i>- You think they're they gone?<i>
                //<i>- That can't be.</i>
                if ((italicBeginTagCount == 3 && italicEndTagCount == 1) && GetNumberOfLines(text) == 2)
                {
                    var newLineIdx = text.IndexOf(Environment.NewLine, StringComparison.Ordinal);
                    var firstLine = text.Substring(0, newLineIdx).Trim();
                    var secondLine = text.Substring(newLineIdx).Trim();

                    if ((StartsAndEndsWithTag(firstLine, beginTag, beginTag) && StartsAndEndsWithTag(secondLine, beginTag, endTag)) ||
                        (StartsAndEndsWithTag(secondLine, beginTag, beginTag) && StartsAndEndsWithTag(firstLine, beginTag, endTag)))
                    {
                        text = text.Replace(beginTag, string.Empty);
                        text = text.Replace(endTag, string.Empty);
                        text = text.Replace("  ", " ").Trim();
                        text = beginTag + text + endTag;
                    }
                }
                text = text.Replace("<i></i>", string.Empty);
                text = text.Replace("<i> </i>", string.Empty);
                text = text.Replace("<i>  </i>", string.Empty);
            }
            return text;
        }

        public static int CountTagInText(string text, string tag)
        {
            int count = 0;
            int index = text.IndexOf(tag, StringComparison.Ordinal);
            while (index >= 0)
            {
                count++;
                index = index + tag.Length;
                if (index >= text.Length)
                    return count;
                index = text.IndexOf(tag, index, StringComparison.Ordinal);
            }
            return count;
        }

        public static int GetNumberOfLines(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            int lines = 1;
            int idx = text.IndexOf('\n');
            while (idx >= 0)
            {
                lines++;
                idx = text.IndexOf('\n', idx + 1);
            }
            return lines;
        }

        public static string[] SplitToLines(string source)
        {
            return source.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        }

        public static string RemoveOpenCloseTags(string source, params string[] tags)
        {

            return TagOpenRegex.Replace(
                source,
                m => tags.Contains(m.Groups[1].Value, StringComparer.OrdinalIgnoreCase) ? string.Empty : m.Value);
        }


        public static bool StartsAndEndsWithTag(string text, string startTag, string endTag)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;
            if (!text.Contains(startTag) || !text.Contains(endTag))
                return false;

            while (text.Contains("  "))
                text = text.Replace("  ", " ");

            var s1 = "- " + startTag;
            var s2 = "-" + startTag;
            var s3 = "- ..." + startTag;
            var s4 = "- " + startTag + "..."; // - <i>...

            var e1 = endTag + ".";
            var e2 = endTag + "!";
            var e3 = endTag + "?";
            var e4 = endTag + "...";
            var e5 = endTag + "-";

            bool isStart = false;
            bool isEnd = false;
            if (text.StartsWith(startTag, StringComparison.Ordinal) || text.StartsWith(s1, StringComparison.Ordinal) || text.StartsWith(s2, StringComparison.Ordinal) || text.StartsWith(s3, StringComparison.Ordinal) || text.StartsWith(s4, StringComparison.Ordinal))
                isStart = true;
            if (text.EndsWith(endTag, StringComparison.Ordinal) || text.EndsWith(e1, StringComparison.Ordinal) || text.EndsWith(e2, StringComparison.Ordinal) || text.EndsWith(e3, StringComparison.Ordinal) || text.EndsWith(e4, StringComparison.Ordinal) || text.EndsWith(e5, StringComparison.Ordinal))
                isEnd = true;
            return isStart && isEnd;
        }

        public static bool IsBetweenNumbers(string s, int position)
        {
            if (string.IsNullOrEmpty(s) || position < 1 || position + 2 > s.Length)
                return false;
            return char.IsDigit(s[position - 1]) && char.IsDigit(s[position + 1]);
        }


        public static string RemoveSpaceBeforeAfterTag(string text, string openTag)
        {
            text = FixUpperTags(text);
            var closeTag = string.Empty;
            switch (openTag)
            {
                case "<i>":
                    closeTag = "</i>";
                    break;
                case "<b>":
                    closeTag = "</b>";
                    break;
                case "<u>":
                    closeTag = "</u>";
                    break;
            }

            if (closeTag.Length == 0 && openTag.Contains("<font "))
                closeTag = "</font>";

            // Open tags
            var open1 = openTag + " ";
            var open2 = Environment.NewLine + openTag + " ";

            // Closing tags
            var close1 = "! " + closeTag + Environment.NewLine;
            var close2 = "? " + closeTag + Environment.NewLine;
            var close3 = " " + closeTag;
            var close4 = " " + closeTag + Environment.NewLine;

            if (text.Contains(close1))
                text = text.Replace(close1, "!" + closeTag + Environment.NewLine);

            if (text.Contains(close2))
                text = text.Replace(close2, "?" + closeTag + Environment.NewLine);

            if (text.EndsWith(close3, StringComparison.Ordinal))
                text = text.Substring(0, text.Length - close3.Length) + closeTag;

            if (text.Contains(close4))
                text = text.Replace(close4, closeTag + Environment.NewLine);

            // e.g: ! </i><br>Foobar
            if (text.StartsWith(open1, StringComparison.Ordinal))
                text = openTag + text.Substring(open1.Length);

            if (text.Contains(open2))
                text = text.Replace(open2, Environment.NewLine + openTag);

            // Hi <i> bad</i> man! -> Hi <i>bad</i> man!
            text = text.Replace(" " + openTag + " ", " " + openTag);
            text = text.Replace(Environment.NewLine + openTag + " ", Environment.NewLine + openTag);

            // Hi <i>bad </i> man! -> Hi <i>bad</i> man!
            text = text.Replace(" " + closeTag + " ", closeTag + " ");
            text = text.Replace(" " + closeTag + Environment.NewLine, closeTag + Environment.NewLine);

            text = text.Trim();
            if (text.StartsWith(open1, StringComparison.Ordinal))
                text = openTag + text.Substring(open1.Length);

            return text;
        }


        public static string FixUpperTags(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            var idx = text.IndexOfAny(UppercaseTags, StringComparison.Ordinal);
            while (idx >= 0)
            {
                var endIdx = text.IndexOf('>', idx + 2);
                if (endIdx < idx)
                    break;
                var tag = text.Substring(idx, endIdx - idx).ToLowerInvariant();
                text = text.Remove(idx, endIdx - idx).Insert(idx, tag);
                idx = text.IndexOfAny(UppercaseTags, StringComparison.Ordinal);
            }
            return text;
        }
        public static string FixTags(string SP)
        {
            if (string.IsNullOrWhiteSpace(SP)||SP.Length<2) return SP;
            var tags = Regex.Matches(SP, @"\s*<.+?>\s*");
            if (tags.Count < 1) return SP;
            if (tags.Count == 1)
                if (tags[0].Length < SP.Length)
                    if (tags[0] != null && tags[0].Value.Contains("/") == false && tags[0].Index < 1)
                        if (tags[0].Value.Contains("font face", StringComparison.OrdinalIgnoreCase) || tags[0].Value.Contains("font color", StringComparison.OrdinalIgnoreCase) || tags[0].Value.Contains("font", StringComparison.OrdinalIgnoreCase))
                            return SP + "</font>";
                        else return SP + "</" + GetTagContent(tags[0].Value) + ">";
                    else
                        if (tags[0] != null && tags[0].Value.Contains("/") && (tags[0].Index + tags[0].Length) >= SP.Length - 1)
                            return "<" + GetTagContent(tags[0].Value) + ">" + SP;
                        else return SP;
                else return SP;
            Match startTag = tags[0];
            if (startTag != null && startTag.Value.Contains("/") == false && startTag.Index < 1)
            {
                bool StartingTagEnded = false;
                for (int i = 1; i < tags.Count; i++) if (tags[i].Value.Contains(GetTagContent(startTag.Value)) && tags[i].Value.Contains("/")) StartingTagEnded = true;
                if (StartingTagEnded == false)

                    if (startTag.Value.Contains("font face", StringComparison.OrdinalIgnoreCase) || startTag.Value.Contains("font color", StringComparison.OrdinalIgnoreCase) || startTag.Value.Contains("font", StringComparison.OrdinalIgnoreCase))
                        return SP + "</font>";
                    else return SP + "</" + GetTagContent(startTag.Value) + ">";

            }
            Match EndTag = tags[tags.Count - 1];
            if (EndTag != null && EndTag.Value.Contains("/") && (EndTag.Index + EndTag.Length) >= SP.Length - 1)
            {
                bool EndTagStarted = false;
                for (int i = tags.Count - 1; i <= 0; i++) if (tags[i].Value.Contains(GetTagContent(EndTag.Value)) && tags[i].Value.Contains("/") == false) EndTagStarted = true;
                if (EndTagStarted == false) return "<" + GetTagContent(EndTag.Value) + ">";
            }
            else return SP;
            return SP;
        }

        public static string CopyTags(string copyfrom,string copyto,bool FIX)
        {
            if (string.IsNullOrWhiteSpace(copyfrom)) return copyto;
            if (string.IsNullOrEmpty(copyto)) return copyto = " ";

            string Spattern = @"\s*<.+?>";
            string Epattern = @"<\s*/.+>\s*";

            bool IshaveAEndTag = false, IsHaveAStartTag = false;

            var Startmatch = Regex.Match(copyfrom, Spattern);
            if (Startmatch.Success) if (Startmatch.Index == 0) IsHaveAStartTag = true;

            var Endmatch_ = Regex.Matches(copyfrom, Epattern);
            Match Endmatch = null;
            if (Endmatch_.Count > 0)
            {
                Endmatch = Endmatch_[Endmatch_.Count-1];
                if (Endmatch.Index + Endmatch.Length >= copyfrom.Length) IshaveAEndTag = true;
            }

            if (IsHaveAStartTag && IshaveAEndTag)
            {
                var C2StartTag = Regex.Match(copyto, Spattern);
                if (C2StartTag.Index < 1)
                {
                    if (C2StartTag.Value.Contains(Startmatch.Value.Replace(" ", "").Replace("/", "").Replace("<", "").Replace(">", "")) == false) copyto = Startmatch.Value + copyto;
                }
                else copyto = Startmatch.Value + copyto;

                var C2EndTag_ = Regex.Matches(copyto, Epattern);
                if (C2EndTag_.Count > 0)
                {
                    var C2EndTag = C2EndTag_[0];
                    if (C2EndTag.Index + C2EndTag.Length >= copyto.Length)
                    {
                        string shavedendmatch=Endmatch.Value.Replace(" ", "").Replace("/", "").Replace("<", "").Replace(">", "");
                        string shavedc2endtag = C2EndTag.Value.Replace(" ", "").Replace("/", "").Replace("<", "").Replace(">", "");
                        if ((shavedc2endtag.Contains(shavedendmatch)||shavedendmatch.Contains(shavedc2endtag))==false) copyto = copyto + Endmatch.Value;

                    }
                    else copyto = copyto + Endmatch.Value;
                }
                else copyto = copyto + Endmatch.Value.Replace(" ", "");
            }
            else return string.Copy(copyto);

            return copyto;
        }

        public static string RemoveDuplicateTags(string str)
        {
            var Bstartingtags = Regex.Matches(str, @"<b>" );
            var Bendingtags = Regex.Matches(str, @"</b>");
            if (Bstartingtags.Count < Bendingtags.Count)
                for (int i = Bendingtags.Count - Bstartingtags.Count; i>0; i--)
                {
                    var matches = Regex.Matches(str,@"</b>");
                    str = str.Remove(matches[matches.Count-1].Index, matches[matches.Count-1].Length);
                }
            
            else if (Bstartingtags.Count > Bendingtags.Count)
                for (int i = Bstartingtags.Count-Bendingtags.Count;i<=Bstartingtags.Count; i++)
                {
                    var match = Bstartingtags[i-1];
                    str=str.Remove(match.Index, match.Length);
                }
            return str;
            
        }

        public static string RemoveJunkTags(string str)
        {
            var matches = Regex.Matches(str, @"<\s{0,3}/");
            foreach (Match item in matches)
            {
                string next = StringTools.SubstrinString(str, item.Index, item.Index + 5, true);
                if (Regex.Match(next, @"b\s{0,3}>").Success || Regex.Match(next, @"i\s{0,3}>").Success || Regex.Match(next, @"u\s{0,3}>").Success || Regex.Match(next, @"font\s{0,3}>").Success) continue;
                else str = str.ReplaceRangeWithChar('\uFFFF', item.Index, item.Index + item.Length);/*str = str.Remove(item.Index, item.Length);*/

            }
            matches = Regex.Matches(str, @"/\s{0,3}>");
            foreach (Match item in matches)
            {
                string last = StringTools.SubstrinString(str, item.Index, item.Index - 5, true);
                if (Regex.Match(last, @"<\s{0,3}b").Success || Regex.Match(last, @"<\s{0,3}i").Success || Regex.Match(last, @"<\s{0,3}u").Success || Regex.Match(last, @"<\s{0,3}u").Success) continue;
                else str = str.ReplaceRangeWithChar('\uFFFF', item.Index, item.Index + item.Length); /*str = str.Remove(item.Index, item.Length);*/
            }
            str=str.Replace("<>", "");
            return str.Replace("\uFFFF", "");
            //return str.Replace("</", "").Replace("/>", "").Replace("<>", "");
        }

        public static string CleanTags(string str)
        {


            var StartingBtags = Regex.Matches(str, @"<\s{0,}b\s{0,}>");//searching for <[any space]b[any space]>
            var EndingBtags = Regex.Matches(str, @"<\s{0,}/\s{0,}b\s{0,}>");//searching for <[any space]/[any space]b[any space]>
            if (EndingBtags.Count < StartingBtags.Count)
            {
                int most = StartingBtags.Count - EndingBtags.Count;
                while (most > 0)
                {
                    str = str + "</b>";
                    most--;
                }
            }
            if (EndingBtags.Count > StartingBtags.Count)
            {
                for (int i = EndingBtags.Count-1; i+1 > StartingBtags.Count; i--)
                {
                    str = str.ReplaceRangeWithChar('\uFFFF',EndingBtags[i].Index,EndingBtags[i].Length);
                }
            }

        str = str.Replace("\uFFFF","");

            return str;
            
        }


        public static string SurroundWithTag(string target,string tag)
        {
            if (string.IsNullOrEmpty(target)) return "";
            if (string.IsNullOrWhiteSpace(tag)) return target;
            if (tag.Equals("font", StringComparison.CurrentCultureIgnoreCase))
            {
                bool isopenningtagexists = false;
                bool isclosingtagexists = false;
                var mat = Regex.Match(target, @"\s*<\s*font.*>");
                if (mat.Success) if (mat.Index == 0) isopenningtagexists = true;
                mat = Regex.Match(target, @"\s*<\s*/\s*font\s*>\s*");
                if (mat.Success) if (mat.Index + mat.Length >= target.Length - 1) isclosingtagexists = true;
                if (!isopenningtagexists) target = "<font>" + target;
                if (!isclosingtagexists) target = target + "</font>";
                return target;

            }
            else
            {
                bool isopeningtagexits = false,isclosingtagexits=false;
                var mat = Regex.Match(target, string.Format(@"\s*<\s*{0}\s*>", tag));
                if (mat.Success) if (mat.Index == 0) isopeningtagexits = true;
                mat = Regex.Match(target, string.Format(@"<\s*/\s*{0}>\s*", tag));
                if (mat.Success) if (mat.Index + mat.Length >= target.Length - 1) isclosingtagexits = true;
                if (!isopeningtagexits) target = "<" + tag + ">";
                if (!isclosingtagexits) target = target + "</"+tag+">";
                return target;
                
            }
                
        }

        public static string[] GetSuroundedtag(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return null;
            var lasttags = Regex.Matches(str, @"<\s{0,}/.+>");
            if (lasttags.Count < 1) return null;
            var lasttag = lasttags[lasttags.Count-1];

            var startingtags = Regex.Matches(str, @"<.+>");
            if (startingtags.Count < 1) return null;
            var starttag = startingtags[0];
            if (GetTagContent(starttag.Value).Equals(GetTagContent(lasttag.Value))) return new string[] { starttag.Value, lasttag.Value };
            else return null;
        }


        public static string GetTagContent(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag)) return "";
            var lessthan = Regex.Match(tag,"<");
            var morethan = Regex.Match(tag, ">");
            if (!(lessthan.Success || morethan.Success)) return "";
            return StringTools.SubstrinString(tag,lessthan.Index+1,morethan.Index,true).Replace("/","");
        }


        public static object Get(string GetFrom, GetWhat getwhat)
        {

            string Spattern = @"\s*<.+?>";
            string Epattern = @"<\s*/.+>\s*";

            if (getwhat == GetWhat.StartingTag)
            {
                var Startmatch = Regex.Match(GetFrom, Spattern);
                return Startmatch.Success && Startmatch.Index == 0 ? Startmatch.Value : null;
            }
            else if (getwhat == GetWhat.EndingTag)
            {
                var Endmatch_ = Regex.Matches(GetFrom, Epattern);
                Match Endmatch = null;
                if (Endmatch_.Count > 0)
                {
                    Endmatch = Endmatch_[Endmatch_.Count - 1];
                    return Endmatch.Index + Endmatch.Length >= GetFrom.Length ? Endmatch.Value : null;
                }
                else return null;
                
            }
            else if (getwhat == GetWhat.SurroundingTag) return GetSuroundedtag(GetFrom);
            else return null;
            
        }

        public static string ToggleTag(string str, string startTag,string Endtag)
        {
            bool HaveSameStartTag=false, HaveSamaeEndTag = false;
            var StartMatch_ = Regex.Matches(str.Trim(), startTag);
            var EndMatch_ = Regex.Matches(str.Trim(), Endtag);
            HaveSameStartTag = StartMatch_.Count > 0 ? StartMatch_[0].Index < 2 ? true : false : false;
            HaveSamaeEndTag = EndMatch_.Count > 0 ? EndMatch_[0].EndIndex() + 1 >= str.Length ? true : false:false;

            Match StartMatch, EndMatch;

            if (HaveSamaeEndTag && HaveSameStartTag)
            {
                StartMatch = StartMatch_[0]; EndMatch = EndMatch_.Last();
                return str.ReplaceRangeWithChar('\uFFFF', StartMatch.Index, StartMatch.EndIndex())
               .ReplaceRangeWithChar('\uFFFF', EndMatch.Index, EndMatch.EndIndex()).Replace("\uFFFF", "");
            }
            else
                return !HaveSamaeEndTag && HaveSameStartTag ? 
                str + Endtag : HaveSamaeEndTag && !HaveSameStartTag ? startTag + str 
                : !HaveSamaeEndTag && !HaveSameStartTag ? startTag+str+Endtag:str;
        }


    } 
    

     enum GetWhat
    {
        StartingTag,EndingTag,SurroundingTag
    }
}
