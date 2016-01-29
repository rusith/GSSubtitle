using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GSSubtitle.Tools
{
    public static class Needs
    {
        public static string LineAlignmentRegexPattern { get; } = @"\{\\\\.{1,3}\}";
        public static string StartTag_RP { get; } = @"<.{2,}>";
    }
}
