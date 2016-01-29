using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSSubtitle.Tools
{
    public class FileTools
    {
        public static bool CheckFileReadable(string filepath)
        {
            try { string text = File.ReadAllText(filepath); return true; }
            catch { return false; }
        }


        public static bool IsPlayableVideo(string filepath)
        {
            if (string.IsNullOrEmpty(filepath)) return false;
            string extention = Path.GetExtension(filepath).ToLower().Trim('.');
            if (string.IsNullOrEmpty(extention)) return false;

            ShellObject obj = ShellObject.FromParsingName(filepath);
            string filetype = "";
              filetype=  obj.Properties.System.MIMEType.Value;
            if (filetype == null) return false;
            if (filetype.Contains("video")) return true;
            else return false;
        }
    }
}
