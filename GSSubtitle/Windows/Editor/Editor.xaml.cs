using GSSubtitle.Controllers.UnReDoPattern;
using GSSubtitle.Models;
using GSSubtitle.Models.SubtitleItems;
using GSSubtitle.Models.SubtitleItems.LineItems;
using GSSubtitle.Tools;
using GSSubtitle.Tools.Extensions;
using GSSubtitle.Tools.SubtitleTools;
using GSSubtitle.Windows.KaroakeEffect;
using GSSubtitle.Windows.Special_Charcters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace GSSubtitle.Windows.Editor
{
    public partial class Editor : Window
    {
        #region Fields

        private Subtitle _CurrentSubtitle;
        private bool palyerslidermousedown = false;
        Thread PlayerControlThred;


        #region ListViewDate(temp)

        private bool MLVIsSomethingSelected = false;
        //private bool MLVIsContextMenuOpened = false;
        private int MLVSelectedItemsCount = 0;


        #endregion

        #endregion

        #region Helper Methods

        /// <summary>
        /// determine given object is a ListView or not
        /// </summary>
        /// <param name="obj">object to determine</param>
        /// <returns>is a listView or not</returns>
        private bool IsLV(object obj)
        {
            return obj.GetType() == typeof(ListView);
        }

        /// <summary>
        /// return given object as ListView type
        /// </summary>
        /// <param name="obj">object to use</param>
        /// <returns>Type Changed Object</returns>
        private ListView AsLV(object obj)
        {
            return obj as ListView;
        }

        private bool IsTB(object obj)
        {
            return obj.GetType() == typeof(TextBox);
        }

        private TextBox AsTB(object obj)
        {
            return obj as TextBox;
        }
        #endregion

        #region Methods

        public Editor()
        {
            InitializeComponent();
        }


        /// <summary>
        /// show an open file dialog,get a file and read it and set CurrentSubtitle Instant
        /// </summary>
        private void OpenSubtitleFile()
        {
            var openfiledialog = new Microsoft.Win32.OpenFileDialog { Title = "select a subtitle file", Filter = "Supported Files| *.srt|All Files|*.*", Multiselect = false };

            if (openfiledialog.ShowDialog() == true)
            {
                var selectedfile = openfiledialog.FileName;
                if (FileTools.CheckFileReadable(selectedfile) == false)
                    if (MessageBox.Show("the file you selected cannot read.\nretry with another file?", "cannot read file!", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                        OpenSubtitleFile();
                    else
                        return;

                else
                {
                    var subtitleformat = SubtitleTools.DetectSubtitleFormat(selectedfile, true);
                    if (subtitleformat == SubtitleFormat.UNKNOWN)
                    {
                        MessageBox.Show("the file you selected is not a correct supported subtitle. \ncannot continue.", "not in correct format", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    else if (subtitleformat == SubtitleFormat.SRT)
                    {
                        _CurrentSubtitle = SrtTools.ReadSrt(selectedfile);
                        //FindAndPlayVideo(openfiledialog.FileName);

                    }

                }

                if (_CurrentSubtitle != null)
                {
                    DataContext = _CurrentSubtitle;
                    mainListView.SelectedIndex = 0;
                }


            }
            else
                return;

        }

        private void RefrshDataContext()
        {
            int selectedindex=mainListView.SelectedIndex;
            if (mainListView.SelectedItems.Count > 0) selectedindex = mainListView.SelectedIndex;
            var currentdatacontext = DataContext;
            DataContext = null;
            DataContext = currentdatacontext;
            mainListView.SelectedIndex = selectedindex;
        }

        //executing on a thread
        private void ControlPlayer(object obj)
        {
            while (editor.Visibility == Visibility.Visible)
            {
                Thread.Sleep(500);
                player.Dispatcher.BeginInvoke((Action)(() =>
                {
                    if (!palyerslidermousedown) playerslider.Value = player.Position.TotalMilliseconds;

                    playerpositiontextblok.Text = player.Position.ToString();
                }));
            }
        }

        private void Pauseplayeraftersometime(double timetosleep, int selectedindex)
        {
            Thread.Sleep(Convert.ToInt32(timetosleep));
            int selectedindexnow = 0;
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate ()
            {
                selectedindexnow = mainListView.SelectedIndex;
            }));
            if (selectedindex == selectedindexnow)
                Debug.WriteLine("index matched");
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate ()
            {
                player.Pause();
            }));
        }

        private void TextBoxListViewToggleTag(string tag,bool UndoAble)
        {

            using (UndoAble ? new PropertyChange(contextEditor, "Text", "Text Changed"):null)
            {
                string text = string.Empty;
                int selectionStart = contextEditor.SelectionStart;

                // No text selected.
                if (contextEditor.SelectedText.Length == 0)
                {
                    text = contextEditor.Text;
                    // Split lines (split a subtitle into its lines).
                    var lines = text.SplitToLines();
                    // Get current line index (the line where the cursor is).
                    int selectedLineNumber = contextEditor.GetLineIndexFromCharacterIndex(contextEditor.SelectionStart);

                    Boolean isDialog = false;
                    int lineNumber = 0;
                    string templine = string.Empty;
                    var lineSb = new StringBuilder();
                    int tagLength = 0;
                    // See if lines start with "-".
                    foreach (var line in lines)
                    {
                        // Append line break in every line except the first one
                        if (lineNumber > 0)
                            lineSb.Append(Environment.NewLine);
                        templine = line;

                        string positionTag = string.Empty;
                        int indexOfEndBracket = templine.IndexOf('}');
                        if (templine.StartsWith("{\\") && indexOfEndBracket > 1 && indexOfEndBracket < 6)
                        {
                            // Find position tag and remove it from string.
                            positionTag = templine.Substring(0, indexOfEndBracket + 1);
                            templine = templine.Remove(0, indexOfEndBracket + 1);
                        }

                        if (templine.StartsWith('-') || templine.StartsWith("<" + tag + ">-"))
                        {
                            isDialog = true;
                            // Apply tags to current line (it is the selected line). Or remove them.
                            if (selectedLineNumber == lineNumber)
                            {
                                // Remove tags if present.
                                if (templine.Contains("<" + tag + ">"))
                                {
                                    templine = templine.Replace("<" + tag + ">", string.Empty);
                                    templine = templine.Replace("</" + tag + ">", string.Empty);
                                    tagLength = -3;
                                }
                                else
                                {
                                    // Add tags.
                                    templine = string.Format("<{0}>{1}</{0}>", tag, templine);
                                    tagLength = 3;
                                }
                            }
                        }
                        lineSb.Append(positionTag + templine);
                        lineNumber++;
                    }
                    if (isDialog)
                    {
                        text = lineSb.ToString();
                        contextEditor.Text = text;
                        contextEditor.SelectionStart = selectionStart + tagLength;
                        contextEditor.SelectionLength = 0;
                    }
                    // There are no dialog lines present.
                    else
                    {
                        // Remove tags if present.
                        if (text.Contains("<" + tag + ">"))
                        {
                            text = text.Replace("<" + tag + ">", string.Empty);
                            text = text.Replace("</" + tag + ">", string.Empty);
                            contextEditor.Text = text;
                            contextEditor.SelectionStart = selectionStart - 3;
                            contextEditor.SelectionLength = 0;
                        }
                        else
                        {
                            // Add tags.
                            int indexOfEndBracket = text.IndexOf('}');
                            if (text.StartsWith("{\\") && indexOfEndBracket > 1 && indexOfEndBracket < 6)
                                text = string.Format("{2}<{0}>{1}</{0}>", tag, text.Remove(0, indexOfEndBracket + 1), text.Substring(0, indexOfEndBracket + 1));
                            else
                                text = string.Format("<{0}>{1}</{0}>", tag, text);
                            contextEditor.Text = text;
                            contextEditor.SelectionStart = selectionStart + 3;
                            contextEditor.SelectionLength = 0;
                        }
                    }
                }
                else
                {
                    string post = string.Empty;
                    string pre = string.Empty;
                    // There is text selected
                    text = contextEditor.SelectedText;
                    while (text.EndsWith(' ') || text.EndsWith(Environment.NewLine) || text.StartsWith(' ') || text.StartsWith(Environment.NewLine))
                    {
                        if (text.EndsWith(' '))
                        {
                            post += " ";
                            text = text.Remove(text.Length - 1);
                        }
                        if (text.EndsWith(Environment.NewLine))
                        {
                            post += Environment.NewLine;
                            text = text.Remove(text.Length - 2);
                        }
                        if (text.StartsWith(' '))
                        {
                            pre += " ";
                            text = text.Remove(0, 1);
                        }
                        if (text.StartsWith(Environment.NewLine))
                        {
                            pre += Environment.NewLine;
                            text = text.Remove(0, 2);
                        }
                    }

                    // Remove tags if present.
                    if (text.Contains("<" + tag + ">"))
                    {
                        text = text.Replace("<" + tag + ">", string.Empty);
                        text = text.Replace("</" + tag + ">", string.Empty);
                    }
                    else
                    {
                        // Add tags.
                        int indexOfEndBracket = text.IndexOf('}');
                        if (text.StartsWith("{\\") && indexOfEndBracket > 1 && indexOfEndBracket < 6)
                        {
                            text = string.Format("{2}<{0}>{1}</{0}>", tag, text.Remove(0, indexOfEndBracket + 1), text.Substring(0, indexOfEndBracket + 1));
                        }
                        else
                        {
                            text = string.Format("<{0}>{1}</{0}>", tag, text);
                        }
                    }
                    // Update text and maintain selection.
                    if (pre.Length > 0)
                    {
                        text = pre + text;
                        selectionStart += pre.Length;
                    }
                    if (post.Length > 0)
                    {
                        text = text + post;
                    }
                    contextEditor.SelectedText = text;
                    contextEditor.SelectionStart = selectionStart;
                    contextEditor.SelectionLength = text.Length;
                }
            }


        }


        private void insertCharctorTocontextEditrSelectionStart(string text)
        {
            contextEditor.Text = contextEditor.Text.Insert(contextEditor.SelectionStart, text);
        }

        //when subtitle is opened find corresponding subtitle and play it in player
        private void FindAndPlayVideo(string filepath)
        {
            var files = Directory.GetFiles(Directory.GetParent(filepath).FullName, "*.*").ToList();
            var derectorys = Directory.GetDirectories(Directory.GetParent(filepath).FullName);
            foreach (var item in derectorys)
            {
                files.AddRange(Directory.GetFiles(item, "*.*").ToList());
            }
            string searchname = Path.GetFileNameWithoutExtension(filepath);
            var sortadlist = LevenshteinDistance.SortListByMatching(searchname, files);
            for (int i = 0; i < 10; i++)
            {
                if (sortadlist.Count < i + 1) return;
                if (FileTools.IsPlayableVideo(sortadlist[i]))
                {
                    PlayVideo(sortadlist[i]);
                    return;
                }

            }
            return;

        }

        //play specific video on the player
        private void PlayVideo(string filepath)
        {
            if (!File.Exists(filepath)) return;
            if (!FileTools.IsPlayableVideo(filepath)) return;
            var playerstatus = ElimetTools.GetMediaStateOfMediaEliment(player);
            if (playerstatus == MediaState.Play || playerstatus == MediaState.Pause) player.Stop();

            player.Source = new Uri(filepath);
            player.Position = TimeSpan.Zero;
            player.Play();
        }

        //selected items text will clear
        private void ClearSelectedItemsText(bool ContextOrContastContext,bool UndoAble)
        {
            Line fl = mainListView.SelectedItems[0] as Line;
            Line ll = mainListView.SelectedItems[mainListView.SelectedItems.Count - 1] as Line;
            using (UndoAble?mainListView.SelectedItems.Count>1? new ChangeCollection(string.Format("{0} Lines  Text Clear Line Number {1} To {2}", 
                   mainListView.SelectedItems.Count,fl.LineNumber,ll.LineNumber)): new ChangeCollection(string.Format(" Line Number {0}  Text Clear", ll.LineNumber)):null) 
            {
                int count = mainListView.SelectedItems.Count;
                int firstindex = mainListView.SelectedIndex;
                int lastindex = mainListView.SelectedIndex + count - 1;

                if (count < 1) return;
                else if (count <= 3)
                {
                    if (ContextOrContastContext)
                        foreach (Line line in mainListView.SelectedItems) line.ClearContext(UndoAble);
                    else
                        foreach (Line line in mainListView.SelectedItems) line.ClearConstantContext(UndoAble);
                }
                else if (count > 3)
                {
                    if (ContextOrContastContext)
                    {
                        MessageBoxResult Messageboxresault = MessageBox.Show(string.Format("Are You Sure ?\nPress Yes to Clear {0} Lines (Line Number {1} to {2}) Text. ", count, firstindex, lastindex), "Confirm Clear Text", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (Messageboxresault == MessageBoxResult.Yes)
                            foreach (Line line in mainListView.SelectedItems) line.ClearContext(UndoAble);
                        else return;
                        return;
                    }
                    else
                    {
                        MessageBoxResult Messageboxresault = MessageBox.Show(string.Format("Are You Sure ?\nPress Yes to Clear {0} Lines (Line Number {1} to {2}) Original Text. ", count, firstindex, lastindex), "Confirm  Clear Original Text", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (Messageboxresault == MessageBoxResult.Yes)
                            foreach (Line line in mainListView.SelectedItems) line.ClearConstantContext(UndoAble);
                        else return;
                        return;
                    }

                }
            }

            
            
            
        }

        private void ReSelectmainListView()
        {
            bool? link = linkwithsubtitle.IsChecked;
            bool? autopause = autostop.IsChecked;
            linkwithsubtitle.IsChecked = false;


            var si = mainListView.SelectedIndex;
            mainListView.SelectedIndex = -1;
            mainListView.SelectedIndex = si;

            linkwithsubtitle.IsChecked = link;
            autostop.IsChecked = autopause;
        }

        private void CopySelectedTextLinesContextToClipboard()
        {
            if (mainListView.SelectedItems.Count > 0)
            {
                string CopyString = "";
                foreach (Line item in mainListView.SelectedItems) CopyString = CopyString + Environment.NewLine + Environment.NewLine + item.Context;
                if (CopyString.Contains(Environment.NewLine + Environment.NewLine)) CopyString = CopyString.Remove(0, (Environment.NewLine + Environment.NewLine).Length - 1);
                try
                {
                    Clipboard.SetText(CopyString);
                }
                catch
                {
                    //some notification
                }
            }
            else return;
        }

        private void PasteTextFromClipboardToSelectedItem(bool UndoAble)
        {
            if (Clipboard.ContainsText())
            {
                if (mainListView.SelectedItems.Count == 1)
                {
                    ((Line)mainListView.SelectedItem).SetContext(Clipboard.GetText(),UndoAble);
                }
                else if (mainListView.SelectedItems.Count > 1)
                {
                    string to = Clipboard.GetText();
                    string[] toto = Regex.Split(to, Environment.NewLine + Environment.NewLine);
                    using (UndoAble ? new ChangeCollection("{0} Lines Text Change", mainListView.SelectedItems.Count+"") : null)
                    {
                        for (int i = 0; i < toto.Length && i < mainListView.SelectedItems.Count; i++)
                        {
                            ((Line)mainListView.SelectedItems[i]).SetContext(toto[i],UndoAble);
                        }
                    }
                        
                }
            }

            
        }

        private void SetSelectedItemContrextAsConstantContext(bool UndoAble)
        {
            if (mainListView.SelectedItems.Count < 1) return;
            using (UndoAble ? new ChangeCollection("Original Text Changed Of {0} Lines", mainListView.SelectedItems.Count + "") : null)
            {
                foreach (Line line in mainListView.SelectedItems) line.SetConstantContext(string.Copy(line.Context),UndoAble) ;
            }
        }

        private void RestoreSelectedItemsOriginalText(bool UndoAble)
        {
            
            if (mainListView.SelectedItems.Count < 1) return;
            using (UndoAble ? new ChangeCollection("{0} Lines Text Reset To Original", mainListView.SelectedItems.Count + "") : null)
            {
                foreach (Line item in mainListView.SelectedItems) item.SetContext(string.Copy(item.ConstantContext),UndoAble);
            }
                
        }

        private void SetMediaSliderValuesWhenMediaElimentSourceChanged()
        {
            if (player.NaturalDuration.HasTimeSpan)
            {
                playerslider.Maximum = player.NaturalDuration.TimeSpan.TotalMilliseconds;
                playerlenthtextblock.Text = player.NaturalDuration.TimeSpan.ToString();
            }
        }

        private void StartPlayerControllerThread()
        {
            PlayerControlThred = new Thread(ControlPlayer);
            PlayerControlThred.Start();
        }

        private void WhenMainListviewSelectionChanged()
        {
            mainListView.ScrollIntoView(mainListView.SelectedItem);

            if (linkwithsubtitle.IsChecked == true)
                if (_CurrentSubtitle != null && mainListView.SelectedItem != null)
                    if (player.HasAudio || player.HasVideo)
                        if (player.NaturalDuration.HasTimeSpan)
                        {
                            if (ElimetTools.GetMediaStateOfMediaEliment(player) == MediaState.Pause) player.Play();

                            double endtimemilliseconds = ((Line)mainListView.SelectedItem).EndTime.ToMilliSeconds();
                            double starttimemilliseconds = ((Line)mainListView.SelectedItem).StartTime.ToMilliSeconds();

                            if (endtimemilliseconds <= player.NaturalDuration.TimeSpan.TotalMilliseconds)
                                if (autostop.IsChecked == true)
                                {
                                    player.Position = ((Line)mainListView.SelectedItem).StartTime.ToTimeSpan();
                                    double timetoplay = endtimemilliseconds - starttimemilliseconds;
                                    int selectedindex = mainListView.SelectedIndex;
                                    if (timetoplay < 300000 && timetoplay < 1 == false) Task.Factory.StartNew(() => Pauseplayeraftersometime(timetoplay, selectedindex));
                                }
                                else player.Position = ((Line)mainListView.SelectedItem).StartTime.ToTimeSpan();
                        }
        }

        private void ChangeMainListViewSelection(int by)
        {
            if (by == 0 || mainListView.Items.Count < 2) return;
            if (mainListView.Items.Count - 1 >= mainListView.SelectedIndex + by) mainListView.SelectedIndex = mainListView.SelectedIndex + by;
        }

        private void StopPlayer()
        {
            var meadistate = ElimetTools.GetMediaStateOfMediaEliment(player);
            if (meadistate == MediaState.Play || meadistate == MediaState.Pause) player.Stop();
        }

        private void OpenMediaForPlay()
        {
            var openfiledialog = new Microsoft.Win32.OpenFileDialog { Title = "select media file you want to play", Multiselect = false };
            if (openfiledialog.ShowDialog() == true)
            {
                PlayVideo(openfiledialog.FileName);
            }
        }

        private void ChangeMLVSelectedLinesFont(System.Drawing.Font font,bool UndoAble)
        {
            if (mainListView.SelectedItems.Count < 1 || font == null) return;
            using (UndoAble ? mainListView.SelectedItems.Count > 1 ? new ChangeCollection( mainListView.SelectedItems.Count+" Lines" +" Font Change to {0}",font.Name) : null:null)
            {
                if (mainListView.SelectedItems.Count > 1)
                {
                    string context,checkstring = "";
                    int end = 0;
                    Match facecheck;
                    foreach (Line line in mainListView.SelectedItems)
                    {
                        context = line.Context;
                        if (context.StartsWith("<font", false, RegexOptions.IgnorePatternWhitespace))
                        {
                            //have some font or font color
                            end = context.IndexOf('>');
                            checkstring = context.SubstrinString(0, end, true);
                            facecheck = Regex.Match(checkstring, "face\\s*=\\s*\".*\"");
                            if (facecheck.Success)
                            {
                                context = context.ReplaceRangeWithChar('\uFFFF', facecheck.Index, facecheck.Index + facecheck.Length - 1);
                                context = context.Insert(facecheck.Index, "face=\"" + font.Name + "\"");
                                context = context.Replace("\uFFFF", "");
                                line.SetContext(context, UndoAble);
                            }
                            else line.SetContext(context.Insert(context.IndexOf("font") + 4, string.Format(" face=\"{0}\"", font.Name)), UndoAble);

                        }
                        else line.SetContext(string.Format("<font face=\"{0}\">{1}</font>", font.Name, context), UndoAble);
                    }
                }
                else
                {
                    Line line = mainListView.SelectedItem as Line;
                    string text = line.Context;
                    if (text.StartsWith("<font", false, RegexOptions.IgnoreCase))
                    {
                        //have some font or font color
                        int end = text.IndexOf('>');
                        var checkstring = text.SubstrinString(0, end, true);
                        var facecheck = Regex.Match(checkstring, "face\\s*=\\s*\".*\"");
                        if (facecheck.Success)
                        {
                            text=text.ReplaceRangeWithChar('\uFFFF', facecheck.Index, facecheck.Index + facecheck.Length - 1);
                            text=text.Insert(facecheck.Index, "face=\"" + font.Name + "\"");
                            text=text.Replace("\uFFFF", "");
                            line.SetContext(text, UndoAble);
                        }
                        else line.SetContext(text.Insert(text.IndexOf("font")+4,string.Format(" face=\"{0}\"",font.Name)),UndoAble);
                    }
                    else line.SetContext(string.Format("<font face=\"{0}\">{1}</font>",font.Name,text),UndoAble);
                }


            }

  
        }
        private void ChangeMLVSelectedLinesFontColor(System.Drawing.Color color,bool UndoAble)
        {
            if (mainListView.SelectedItems.Count < 1||color==null) return;
            using (UndoAble ? mainListView.SelectedItems.Count > 1 ? new ChangeCollection("{0} Lines Font Change ",mainListView.SelectedItems.Count+""):null:null)
            {
                if (mainListView.SelectedItems.Count > 1)
                {
                    string text, checkstring = "";
                    int end = 0;
                    Match colormatch;

                    foreach (Line line in mainListView.SelectedItems)
                    {
                        text = line.Context;
                        if (text.StartsWith("<font", false, RegexOptions.IgnoreCase))
                        {
                            end = text.IndexOf('>');
                            checkstring = text.SubstrinString(0, end, true);
                            colormatch = Regex.Match(checkstring, "color\\s*=\\s*\".*\"");
                            if (colormatch.Success)
                            {
                                text = text.ReplaceRangeWithChar('\uFFFF', colormatch.Index, colormatch.Length - 1);
                                text = text.Insert(colormatch.Index, "color=\"" + color.ToHex() + "\"");
                                text = text.Replace("\uFFFF", "");
                                line.SetContext(text, UndoAble);
                            }
                            else line.SetContext(text.Insert(text.IndexOf("font")+4,string.Format("color=\"{0}\"",color.ToHex())),UndoAble);


                        }
                        else line.SetContext(string.Format("<font color=\"{0}\">{1}</font>", color.ToHex(), text), UndoAble);
                    }
                }
                else
                {
                    
                    Line line = mainListView.SelectedItem as Line;

                    string text = line.Context;
                    if (text.StartsWith("<font", false, RegexOptions.IgnoreCase))
                    {
                        int end = text.IndexOf('>');
                        string checkstring = text.SubstrinString(0, end, true);
                        var colormatch = Regex.Match(checkstring, "color\\s*=\\s*\".*\"");
                        if (colormatch.Success)
                        {
                            text = text.ReplaceRangeWithChar('\uFFFF', colormatch.Index, colormatch.Length - 1);
                            text = text.Insert(colormatch.Index, "color=\"" + color.ToHex() + "\"");
                            text = text.Replace("\uFFFF", "");
                            line.SetContext(text, UndoAble);
                        }
                        else line.SetContext(text.Insert(text.IndexOf("font") + 4, string.Format("color=\"{0}\"", color.ToHex())), UndoAble);


                    }
                    else line.SetContext(string.Format("<font color=\"{0}\">{1}</font>", color.ToHex(), text), UndoAble);

                }
            }

        }
        private void ChangeContextEditorFont()
        {
            string selectedtext = contextEditor.SelectedText;
            int selectionstart = contextEditor.SelectionStart;

            System.Windows.Forms.FontDialog fontdialog = new System.Windows.Forms.FontDialog();
            if (fontdialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                bool done = false;
                if (selectedtext.StartsWith("<font "))
                {
                    int end = selectedtext.IndexOf('>');
                    if (end > 0)
                    {
                        string font = selectedtext.Substring(0, end);
                        if (font.Contains(" color=") && !font.Contains("face="))
                        {
                            var start = selectedtext.IndexOf(" color=", StringComparison.Ordinal);
                            selectedtext = selectedtext.Insert(start, string.Format(" face=\"{0}\"", fontdialog.Font.Name));
                            done = true;
                        }
                        else if (font.Contains(" face="))
                        {
                            int facestart = font.IndexOf(" face=", StringComparison.Ordinal);
                            if (selectedtext.IndexOf('"', facestart + " face=".Length + 1) > 0)
                                end = selectedtext.IndexOf('"', facestart + " face=".Length + 1);
                            selectedtext = selectedtext.Substring(0, facestart) + string.Format(" face=\"{0}", fontdialog.Font.Name) + selectedtext.Substring(end);
                            done = true;
                        }


                    }
                }
                if (!done)
                    selectedtext = string.Format("<font face=\"{0}\">{1}</font>", fontdialog.Font.Name, selectedtext);
                contextEditor.SelectedText = selectedtext;
                contextEditor.SelectionStart = selectionstart;
                contextEditor.SelectionLength = selectedtext.Length;
            }
        }

        private void RevertContextEditorToNormal(bool UndoAble)
        {
             using (UndoAble?new PropertyChange(contextEditor, "Text", string.Format("Line Number {0} Text Normalized",((Line)mainListView.SelectedItem).LineNumber)):null)
            {
                string text = contextEditor.SelectedText;
                int selectionStart = 0;
                if (String.IsNullOrEmpty(text))
                {
                    text = contextEditor.Text;
                    selectionStart = 0;
                    contextEditor.Text = HtmlTools.RemoveHtmlTags(text);
                    contextEditor.SelectAll();
                }
                else
                {
                    selectionStart = contextEditor.SelectionStart;
                    text = HtmlTools.RemoveHtmlTags(text);
                    contextEditor.SelectedText = text;
                    contextEditor.SelectionStart = selectionStart;
                    contextEditor.SelectionLength = text.Length;
                }
            }

        }

        // if player is playing --> pause 
        //              paused-->play
        private void PlayOrPausePlayer()
        {
            var playerstate = ElimetTools.GetMediaStateOfMediaEliment(player);
            switch (playerstate)
            {
                case MediaState.Play:
                    player.Pause();
                    playerImage.Source = ElimetTools.BitmapImageFromString("/Images/Pause.png");
                    break;

                case MediaState.Pause:
                    player.Play();
                    playerImage.Source = ElimetTools.BitmapImageFromString("/Images/Play.png");
                    break;
            }

            Storyboard secstorybord = this.FindResource("playerImageAnimation") as Storyboard;
            secstorybord.Begin();
        }

        private void ChangeCoontextEditorFontColor()
        {
            string text = contextEditor.SelectedText;
            int selectionStart = contextEditor.SelectionStart;


            System.Windows.Forms.ColorDialog colordialog = new System.Windows.Forms.ColorDialog();
            if (colordialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string color = StringTools.ColorToHex(colordialog.Color);
                bool done = false;
                string s = text;
                if (s.StartsWith("<font "))
                {
                    int end = s.IndexOf('>');
                    if (end > 0)
                    {
                        string f = s.Substring(0, end);
                        if (f.Contains(" face=") && !f.Contains(" color="))
                        {
                            var start = s.IndexOf(" face=", StringComparison.Ordinal);
                            s = s.Insert(start, string.Format(" color=\"{0}\"", color));
                            text = s;
                            done = true;
                        }
                        else if (f.Contains(" color="))
                        {
                            int colorStart = f.IndexOf(" color=", StringComparison.Ordinal);
                            if (s.IndexOf('"', colorStart + " color=".Length + 1) > 0)
                                end = s.IndexOf('"', colorStart + " color=".Length + 1);
                            s = s.Substring(0, colorStart) + string.Format(" color=\"{0}", color) + s.Substring(end);
                            text = s;
                            done = true;
                        }
                    }
                }

                if (!done)
                    text = string.Format("<font color=\"{0}\">{1}</font>", color, text);

                contextEditor.SelectedText = text;
                contextEditor.SelectionStart = selectionStart;
                contextEditor.SelectionLength = text.Length;
            }
        }

        private void InsertEmptyLineBeforeSelectedLine(bool UndoAble)
        {
            int selectedinde = mainListView.SelectedIndex;
            ((LineList)mainListView.ItemsSource).AddEmptyLine(mainListView.SelectedIndex, true, UndoAble);
            RefrshDataContext();
            mainListView.SelectedIndex = selectedinde;
        }

        private void InsertEmptyLineAfterSelectedLine(bool UndoAble)
        {
            int selectedindex = mainListView.SelectedIndex;
            ((LineList)mainListView.ItemsSource).AddEmptyLine(mainListView.SelectedIndex, false, UndoAble);
            RefrshDataContext();
            mainListView.SelectedIndex = selectedindex + 1;
        }

        private void AddNewLine(bool UndoAble,int count = 1)
        {
            if (_CurrentSubtitle == null) return;
            var lines = (LineList)mainListView.ItemsSource;
            for (int i = 0; i < count; i++)
            {
                if (lines.Count > 0)
                {

                    Line line = new Line { Context = "", ConstantContext = "", LineNumber = lines.Last().LineNumber + 1 };
                    LineList.CopyTwoLineTimes(lines.Last().EndTime, line.StartTime);
                    line.Duration = 0.25;
                    lines.Add(line, UndoAble);
                }
                else
                {
                    Line line = new Line { ConstantContext = "", Context = "", LineNumber = 1 };
                    line.StartTime = new LineTime { MilliSeconds = 0, Seconds = 0, Minutes = 0, Hours = 0 };
                    line.Duration = 0.25;
                    lines.Add(line, UndoAble);
                }
            }



            RefrshDataContext();
            mainListView.SelectedItem = ((LineList)mainListView.ItemsSource).Last();
        }

        private void DeleteSelectedLines(bool UndoAble)
        {
            if (mainListView.SelectedItems.Count < 1) return;
            var resualt= MessageBox.Show(string.Format("Are you sure ? \nare you want to delete {0} lines from the subtitle ?", mainListView.SelectedItems.Count.ToString()),"Please Confirm Delete",MessageBoxButton.YesNo,MessageBoxImage.Question,MessageBoxResult.No);

            if (resualt == MessageBoxResult.Yes)
            {
                int selectedindex = mainListView.SelectedIndex;
                using (UndoAble ? new ChangeCollection("Delete {0} Lines.", mainListView.SelectedItems.Count + "") : null)
                {
                    foreach (var item in mainListView.SelectedItems) ((LineList)mainListView.ItemsSource).Remove((Line)item, UndoAble);
                }
                RefrshDataContext();
                mainListView.SelectedIndex = selectedindex;
            }
            else return;
            
        }

        private void SubScriptContextEditorSelectedText()
        {
            if (string.IsNullOrWhiteSpace(contextEditor.SelectedText)) return;
            contextEditor.SelectedText = StringTools.ToSubscript(contextEditor.SelectedText);
            
        }

        private void SuperscriptContextEditorSelecctedText()
        {
            if (string.IsNullOrWhiteSpace(contextEditor.SelectedText)) return;
            contextEditor.SelectedText = StringTools.ToSuperscript(contextEditor.SelectedText);
        }

        private void SplitSelectedLine(bool UndoAble)
        {
            using (UndoAble ? new ChangeCollection(" Line Number {0} Split", ((Line)mainListView.SelectedItem).LineNumber+"") : null)
            {
                if (mainListView.SelectedItems.Count < 1) return;
                int selectedindex = mainListView.SelectedIndex;
                var SelectedLine = mainListView.SelectedItem as Line;
                if (SelectedLine == null) return;
                string[] splittedText = StringTools.SplitText(SelectedLine.Context); //split selected Line Context To Two Strings
                double time = 0;
                if ((SelectedLine.Duration * 1000) < 2) time = 50 / 1000;
                else time = (SelectedLine.Duration * 1000) / 2;

                Line line1 = new Line();
                LineList.CopyTwoLineTimes(SelectedLine.StartTime, line1.StartTime);
                line1.Duration = time / 1000;
                line1.Context = splittedText[0];
                line1.ConstantContext = splittedText[0];
                line1.LineNumber = SelectedLine.LineNumber;


                Line line2 = new Line();
                LineList.CopyTwoLineTimes(line1.EndTime, line2.StartTime);
                line2.Duration = time / 1000;
                line2.ConstantContext = splittedText[1];
                line2.Context = splittedText[1];
                line2.LineNumber = line1.LineNumber + 1;


                var lines = ((LineList)mainListView.ItemsSource);
                lines.RemoveAt(selectedindex,UndoAble);
                lines.RemoveAt(selectedindex, UndoAble);
                lines.InsertRange(selectedindex, new LineList { line1, line2 },UndoAble);

                RefrshDataContext();
                mainListView.SelectedIndex = selectedindex;
            }

        }

        private void MargeSelectedLines(bool UndoAble)
        {
            using (UndoAble ? new ChangeCollection("{0} Lines Merged. Line Number {1} To {2}", mainListView.SelectedItems.Count + "", (mainListView.SelectedIndex + mainListView.SelectedItems.Count - 1) + ""):null)
            {
                if (mainListView.SelectedItems.Count < 1 || mainListView.SelectedItems.Count > 10) return;
                int FirstIndex = mainListView.SelectedIndex;
                int SlectedItemsCount = mainListView.SelectedItems.Count;
                int LastIndex = (FirstIndex + SlectedItemsCount) - 1;
                Line FirstLine = mainListView.SelectedItem as Line;
                Line LastLine = mainListView.Items[LastIndex - 1] as Line;

                string LastString = "";
                LineList ItemSource = mainListView.ItemsSource as LineList;
                foreach (Line item in mainListView.SelectedItems)
                {

                    LastString = LastString + "</br></br>" + item.Context;
                    ItemSource.Remove(item, UndoAble);
                }
                if (LastString.StartsWith("</br></br>")) LastString = LastString.Remove(0, "</br></br>".Length);



                Line line = new Line { Context = LastString, ConstantContext = LastString, LineNumber = FirstLine.LineNumber };
                LineList.CopyTwoLineTimes(FirstLine.StartTime, line.StartTime);
                LineList.CopyTwoLineTimes(LastLine.EndTime, line.EndTime);
                ItemSource.Insert(FirstIndex, line, UndoAble);
                ((LineList)mainListView.ItemsSource).FixLineNumberSequence(FirstIndex, true);
                RefrshDataContext();
                mainListView.SelectedIndex = FirstIndex;
            }
        }

        private void RevertSelectedLineContextToNormal(bool UndoAble)
        {
            if (mainListView.SelectedIndex > -1)
            {
                using (UndoAble ? new ChangeCollection("{0} Lines Text Normalize ", mainListView.SelectedItems.Count + ""):null)
                {
                    foreach (Line line in mainListView.SelectedItems) line.SetContext(HtmlTools.RemoveHtmlTags(line.Context), UndoAble);
                    var se = mainListView.SelectedIndex;
                    RefrshDataContext();
                    mainListView.SelectedIndex = se;
                }

            }
        }

        private void SyncSelectedItemsTextAndOriginalText(bool UndoAble)
        {
            if (mainListView.SelectedItems.Count < 1) return;

            string text = "";
            using (UndoAble ? new ChangeCollection("{0} Lines Text <-> original Text", mainListView.SelectedItems.Count+"") : null)
            {
                foreach (Line line in mainListView.SelectedItems)
                {
                    text = string.Copy(line.Context);
                    line.SetContext(string.Copy(line.ConstantContext),UndoAble);
                    line.SetConstantContext(string.Copy(text),UndoAble);
                }
            }

        }

        private void TogleMLVSelectedItemsTags(string startTag,string EndTag,bool UndoAble)
        {
           if (mainListView.SelectedItems.Count > 0)
                using (UndoAble ? new ChangeCollection("{0} Lines Text Bold", mainListView.SelectedItems.Count + "") : null)
                {
                    foreach (Line line in mainListView.SelectedItems) line.SetContext(HtmlTools.ToggleTag(line.Context, startTag, EndTag), UndoAble);
                }         
           else return;
        }

        private System.Drawing.Font GetFontUseingFontDialog()
        {
            System.Windows.Forms.FontDialog fdialog = new System.Windows.Forms.FontDialog();
            if (fdialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return fdialog.Font;
            }
            else
            { return null; }
        }

        private void SurroundMainListViewSelectedItemsWithSpecialCharacters(bool UndoAble)
        {
            if (mainListView.SelectedItems.Count < 1) return;
            StringBuilder grabber = new StringBuilder();
            Special_Charactors specialcharactors = new Special_Charactors(grabber);
            if (specialcharactors.ShowDialog() != true) return;
            string text = grabber.ToString();
            if (string.IsNullOrWhiteSpace(text)) return;
            using (UndoAble ? mainListView.SelectedItems.Count > 1 ? new ChangeCollection("{0} Lines Surrounded with {1} ", mainListView.SelectedItems.Count + "", text) : null:null)
            {
                if (mainListView.SelectedItems.Count > 1)
                    foreach (Line item in mainListView.SelectedItems) item.SetContext(string.Format("{0}{1}{2}",text,item.Context,text),UndoAble);
                else
                {
                    Line line = mainListView.SelectedItem as Line;
                    line.SetContext(string.Format("{0}{1}{2}", text, line.Context, text), UndoAble, string.Format("Line Number {0} Text Decorate", line.LineNumber + ""));
                }
            }
        }
       
        /// <summary>
        /// change selected lines alignments 
        /// </summary>
        /// <param name="alignment">TL=TopLeft BR=BottomRight ....</param>
        private void ChangeSelectedLinesAlignment(string alignment,bool UndoAble)
        {
            using (UndoAble ? mainListView.SelectedItems.Count > 1 ? new ChangeCollection("{0} Lines Alignment Changed", mainListView.SelectedItems.Count + "") : null : null)
            {
                string alignmenttext = "";
                switch (alignment)
                {
                    case "TL": alignmenttext = "{\\an7}"; break;
                    case "ML": alignmenttext = "{\\an4}"; break;
                    case "BL": alignmenttext = "{\\an1}"; break;
                    case "TC": alignmenttext = "{\\an8}"; break;
                    case "MC": alignmenttext = "{\\an5}"; break;
                    case "BC": alignmenttext = "{\\an2}"; break;
                    case "TR": alignmenttext = "{\\an9}"; break;
                    case "MR": alignmenttext = "{\\an6}"; break;
                    case "BR": alignmenttext = "{\\an3}"; break;
                }

                if (mainListView.SelectedItems.Count > 1)
                {

                    Match CurrentAlignment;
                    string context = "";
                    foreach (Line line in mainListView.SelectedItems)
                    {
                        context = line.Context;
                        CurrentAlignment = Regex.Match(context, Needs.LineAlignmentRegexPattern);
                        context=CurrentAlignment.Success? alignmenttext + context.ReplaceRangeWithChar('\uFFFF', CurrentAlignment.Index, CurrentAlignment.Index + CurrentAlignment.Length - 1).Replace("\uFFFF", "")
                                : alignmenttext + context;

                        line.SetContext(context, UndoAble);
                    }
                }
                else
                {
                    var line = mainListView.SelectedItem as Line;
                    string context = line.Context;
                    var CurrentAlignment = Regex.Match(context, Needs.LineAlignmentRegexPattern);
                    context = CurrentAlignment.Success ? alignmenttext + context.ReplaceRangeWithChar('\uFFFF', CurrentAlignment.Index, CurrentAlignment.Index + CurrentAlignment.Length - 1).Replace("\uFFFF", "")
                            : alignmenttext + context;

                    line.SetContext(context, UndoAble,string.Format("Line Number {0} Alignment Changed",line.LineNumber));
                }
            }

        }
        #endregion

        #region Events

        private void openfileMenuitem_Click(object sender, RoutedEventArgs e)
        {
            OpenSubtitleFile();
        }

        private void startTimeEditorupbutton_Click(object sender, RoutedEventArgs e)
        {

            startTimeEditor.Text= LineTime.NewFromString(startTimeEditor.Text).AddSomeMilliSeconds(100).ToString();
            startTimeEditor.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void startTimeEditordownbutton_Click(object sender, RoutedEventArgs e)
        {
            startTimeEditor.Text = LineTime.NewFromString(startTimeEditor.Text).AddSomeMilliSeconds(-100).ToString();
            startTimeEditor.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void durationEditorupbutton_Click(object sender, RoutedEventArgs e)
        {
            durationEditor.Text = (((Convert.ToDouble(durationEditor.Text) * 1000) + 100) / 1000).ToString();
        }

        private void durationEditordownbutton_Click(object sender, RoutedEventArgs e)
        {
            durationEditor.Text = (((Convert.ToDouble(durationEditor.Text) * 1000) - 100) / 1000).ToString();

        }

        private void playerchildren_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
        }

        private void playerchildren_Drop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (data.Length < 1) return;
            PlayVideo(data[0]);
        }

        private void playerchildren_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PlayOrPausePlayer();

        }

        private void player_MediaOpened(object sender, RoutedEventArgs e)
        {
            SetMediaSliderValuesWhenMediaElimentSourceChanged();
        }

        private void editor_Initialized(object sender, EventArgs e)
        {
            StartPlayerControllerThread();
        }

        private void playerslider_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            palyerslidermousedown = true;
        }

        private void playerslider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            player.Position = TimeSpan.FromMilliseconds(playerslider.Value);
            palyerslidermousedown = false;
        }

        private void mainListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WhenMainListviewSelectionChanged();
        }

        private void leftToRightControllerCharactorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            insertCharctorTocontextEditrSelectionStart("\u8207");
        }

        private void rightToLeftControllerCharacterMenuItem_Click(object sender, RoutedEventArgs e)
        {
            insertCharctorTocontextEditrSelectionStart("\u8206");
        }

        private void startOfLeftToRightEmbeddingControllerCharacterMenuItem_Click(object sender, RoutedEventArgs e)
        {
            insertCharctorTocontextEditrSelectionStart("\u202A");
        }

        private void startOfLeftTorightOverrrideControlerCharacterMenuItem_Click(object sender, RoutedEventArgs e)
        {
            insertCharctorTocontextEditrSelectionStart("\u202D");
        }

        private void startOfRightToLeftEmbeddingControllerCharcterMenuItem_Click(object sender, RoutedEventArgs e)
        {
            insertCharctorTocontextEditrSelectionStart("\u202B");
        }

        private void startOfRightToLeftOverrideControllerCharactorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            insertCharctorTocontextEditrSelectionStart("\u202E");

        }

        private void autostop_Checked(object sender, RoutedEventArgs e)
        {
            if (linkwithsubtitle.IsChecked == false) linkwithsubtitle.IsChecked = true;
        }

        private void player_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            System.Windows.MessageBox.Show(e.ErrorException.Message, "cannot play selected file", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void jumptonextimage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChangeMainListViewSelection(1);
        }

        private void jumptopreviousimage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChangeMainListViewSelection(-1);
        }

        private void linkwithsubtitle_Unchecked(object sender, RoutedEventArgs e)
        {
            if (autostop.IsChecked == true) autostop.IsChecked = false;
            var vs = ElimetTools.GetMediaStateOfMediaEliment(player);
            if (vs == MediaState.Pause) player.Play();
        }

        private void fillmediaradiobutton_Checked(object sender, RoutedEventArgs e)
        {
            if (fillmediaradiobutton.IsChecked == true) player.Stretch = Stretch.Fill;
        }

        private void uniformmediaradiobutton_Checked(object sender, RoutedEventArgs e)
        {
            if (uniformmediaradiobutton.IsChecked == true) player.Stretch = Stretch.Uniform;
        }

        private void stopmediamenuitem_Click(object sender, RoutedEventArgs e)
        {
            StopPlayer();
        }

        private void openmediamenuitem_Click(object sender, RoutedEventArgs e)
        {
            OpenMediaForPlay();
        }

        //private void contexteditorfontmenuitem_Click(object sender, RoutedEventArgs e)
        //{

        //    ChangeContextEditorFont();
        //}

        //private void contexteditorfontcolormenuitem_Click(object sender, RoutedEventArgs e)
        //{
        //    ChangeCoontextEditorFontColor();
        //}



        //private void contexteditoritalicmenuitem_Click(object sender, RoutedEventArgs e)
        //{
        //    TextBoxListViewToggleTag("i",true);
        //}

        //private void contexteditorundelinemenuitem_Click(object sender, RoutedEventArgs e)
        //{
        //    TextBoxListViewToggleTag("u",true);
        //}

        private void AddSpecialCharacterToCE()
        {
            StringBuilder stringbuilder = new StringBuilder("");
            Special_Charactors specialchars = new Special_Charactors(stringbuilder);
            if (specialchars.ShowDialog() == true)
            {
                int selectionstart = contextEditor.SelectionStart;
                contextEditor.Text = contextEditor.Text.Insert(selectionstart, specialchars.returncharactors.ToString());
            }
        }

        //private void fromClipboardMenuItem_Click(object sender, RoutedEventArgs e)
        //{
        //    PasteTextFromClipboardToSelectedItem(true);
        //}

        //private void setAsOriginalTextMenuItem_Click(object sender, RoutedEventArgs e)
        //{
        //    SetSelectedItemContrextAsConstantContext(true);
        //}

        private void ContextEditorSubscriptTextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SubScriptContextEditorSelectedText();
        }

        private void ContextEditorSuperscriptMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SuperscriptContextEditorSelecctedText();
        }

        //private void splitMenuItem_Click(object sender, RoutedEventArgs e)
        //{
        //    SplitSelectedLine(true);
        //}

        private void button_Click(object sender, RoutedEventArgs e)
        {
            KaroakeEffectWindow karoke = new KaroakeEffectWindow(_CurrentSubtitle, mainListView.SelectedItems.Cast<Line>().ToList(), "Let's Karaoke");
            karoke.Show();
            
        }

        private void mainListView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            //MLVIsContextMenuOpened = true;
            MLVIsSomethingSelected = mainListView.IsSomethingSelected();
            MLVSelectedItemsCount = mainListView.SelectedItemsCount();
        }

        private void mainListView_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            //MLVIsContextMenuOpened = false;
        }

        #endregion

        #region Commands

        private void MargeCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (IsLV(e.Source))
            {
                int sic = AsLV(e.Source).SelectedItems.Count;
                e.CanExecute = sic > 1 && sic <= 10 ? true : false;
            }
            e.Handled = true;
        }

        private void MargeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MargeSelectedLines(true);
            e.Handled = true;
        }

        private void NormalText_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (IsLV(e.Source))
            {
                e.CanExecute = MLVIsSomethingSelected;
            }
            else if (IsTB(e.Source))
            {
                e.CanExecute = AsTB(e.Source).Text.Length > 0 ? true : false;
            }
            else e.CanExecute = false;
            e.Handled = true;
        }

        private void NoremalText_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (contextEditor.IsFocused)
            {
                RevertContextEditorToNormal(true);
                e.Handled = true;
            }
            else if (mainListView.SelectedIndex > -1)
            {
                RevertSelectedLineContextToNormal(true);
                e.Handled = true;
                
            }
            else
            {
                e.Handled = true;
                return;
            }

        }

        private void AddLineCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _CurrentSubtitle != null ? true : false;
            e.Handled = true;

        }

        private void AddLineCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            AddNewLine(true);
            e.Handled = true;
        }

        private void InsetLineCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute =MLVSelectedItemsCount==1?true:false;
            e.Handled = true;
        }

        private void InserLineBeforeCurrentLineCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            InsertEmptyLineBeforeSelectedLine(true);
            e.Handled = true;
        }

        private void InsertLineAfterCurrentLineCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            InsertEmptyLineAfterSelectedLine(true);
            e.Handled = true;
        }

        private void DeleteLinesCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_CurrentSubtitle != null)
            {
                if (IsLV(e.Source))
                {
                    e.CanExecute = MLVIsSomethingSelected;
                    if (MLVIsSomethingSelected) deletelineMenuItem.Header = mainListView.SelectedItems.Count > 1 ? "Delete " + mainListView.SelectedItems.Count + " Lines" : "Delete Line";
                }
                //another source
            }
            else e.CanExecute = false;
            e.Handled = true;
        }

        private void DeleteLineCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DeleteSelectedLines(true);
            e.Handled = true;
        }

        private void TextMenuItemCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MLVIsSomethingSelected;
            e.Handled = true;
        }

        private void ClearTextCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (IsLV(e.Source)) e.CanExecute = true;
            //another source
            e.Handled = true;
        }

        private void ClearTextCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            ClearSelectedItemsText(true,true);
            e.Handled = true;
        }

        private void CLearOriginalTextCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (IsLV(e.Source)) e.CanExecute = true;
            //another source
            e.Handled = true;
        }

        private void CLearOriginalTextCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ClearSelectedItemsText(false,true);
            e.Handled = true;
        }

        private void CopyToClipboardCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (IsLV(e.Source)) e.CanExecute = true;
            //another source
            e.Handled = true;
        }

        private void CopyToClipboardCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            CopySelectedTextLinesContextToClipboard();//cannot undo
            e.Handled = true;
        }

        private void PasteTextFromClipboard_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (IsLV(e.Source))
                try { e.CanExecute = Clipboard.ContainsText(); }
                catch { e.CanExecute = false; }
            //other sources
            e.Handled = true;

        }

        private void PasteTextFromClipboard_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            PasteTextFromClipboardToSelectedItem(true);
        }

        private void SetTextAsOriginalTextCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (IsLV(e.Source)) e.CanExecute = true;
            //another source
            e.Handled = true;
        }

        private void SetTextAsOriginalTextCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SetSelectedItemContrextAsConstantContext(true);
            e.Handled = true;
        }

        private void SetOriginalTextAsTextCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (IsLV(e.Source)) e.CanExecute = true;
            //another source
            e.Handled = true;
        }

        private void SetOriginalTextAsTextCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RestoreSelectedItemsOriginalText(true);
            e.Handled = true;
        }

        private void SyncOriginalTextAndTextCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (IsLV(e.Source)) e.CanExecute = true;
            //another sources
            e.Handled = true;
        }

        private void SyncOriginalTextAndText_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SyncSelectedItemsTextAndOriginalText(true);
            e.Handled = true;
        }

        private void SplitLSelectedLinesCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (IsLV(e.Source)) e.CanExecute = true;
            //another source settings
            e.Handled = true;
        }

        private void SplitLSelectedLinesCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SplitSelectedLine(true);
            e.Handled = true;
        }

        private void BoldTextCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (IsLV(e.Source)) e.CanExecute = MLVIsSomethingSelected;
            else if (IsTB(e.Source))
            {
                //must be the ContextEditor TextBox

                e.CanExecute = contextEditor.Text.Length > 0 ? true : false;
            }
            e.Handled = true;
        }

        private void BoldTextCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            if (IsLV(e.Source))
            {
               TogleMLVSelectedItemsTags("<b>", "</b>", true);  
            }
            else if (IsTB(e.Source)) TextBoxListViewToggleTag("b",true);


            e.Handled = true;
        }

        private void ItalicTextCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            
                
        }

        private void ItalicTextCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LineList lines = new LineList();

        }

        private void UndoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

            e.CanExecute = URD.CanUndo;
            e.Handled = true;
        }

        private void UndoCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Change change = URD.NextUndo;

            URD.UndoLastChnage();
            if (change is ListChange || change is ChangeCollection) RefrshDataContext();
            e.Handled = true;
        }

        private void RedoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = URD.CanRedo;
            e.Handled = true;
        }

        private void RedoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //if (URP.CanRedo) URP.Redo();
            e.Handled = true;
        }

        private void editor_Closed(object sender, EventArgs e)
        {
            PlayerControlThred.Abort();
        }

        private void SurroundWithTagCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (IsLV(e.Source)) e.CanExecute = MLVIsSomethingSelected;
            else if (IsTB(e.Source))
            {
                //must be the ContextEditor TextBox

                e.CanExecute = contextEditor.Text.Length > 0 ? true : false;
            }
            e.Handled = true;
        }

        private void SurroundWithTagCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (IsLV(e.Source))
            {
                switch (e.Parameter as string)
                {
                    case "i":
                        TogleMLVSelectedItemsTags("<i>", "</i>", true);
                        break;
                    case "b":
                        TogleMLVSelectedItemsTags("<b>", "</b>", true);
                        break;
                    case "u":
                        TogleMLVSelectedItemsTags("<u>", "</u>", true);
                        break;
                    case "font":
                        {
                            var font = GetFontUseingFontDialog();
                            if (font == null) return;
                            ChangeMLVSelectedLinesFont(font, true);
                            break;
                        }
                    case "fontcolor":
                        {
                            System.Windows.Forms.ColorDialog colordialog = new System.Windows.Forms.ColorDialog { AnyColor = true, AllowFullOpen = true };

                            if (colordialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                ChangeMLVSelectedLinesFontColor(colordialog.Color, true);
                            break;
                        }
                }

            }
            else if (IsTB(e.Source))
            {
                switch (e.Parameter as string)
                {
                    case "i":
                        TextBoxListViewToggleTag("i", true);
                        break;
                    case "b":
                        TextBoxListViewToggleTag("b", true);
                        break;
                    case "u":
                        TextBoxListViewToggleTag("u", true);
                        break;
                    case "font":
                        ChangeContextEditorFont();
                        break;
                    case "fontcolor":
                        ChangeCoontextEditorFontColor();
                        break;

                }
            }

            e.Handled = true;
        }

        private void AddSpecialCharacterCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void AddSpecialCharacterCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (IsTB(e.Source))
                AddSpecialCharacterToCE();

        }

        private void AddControlCharacterCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (IsTB(e.Source))
                e.CanExecute = true;
        }

        private void AddControlCharacterCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (IsTB(e.Source))
            {
                switch (e.Parameter as string)
                {

                    case "LTR": insertCharctorTocontextEditrSelectionStart("\u8207"); break;
                    case "RTL": insertCharctorTocontextEditrSelectionStart("\u8206"); break;
                    case "SOLTRE": insertCharctorTocontextEditrSelectionStart("\u202A"); break;
                    case "SOLTRO": insertCharctorTocontextEditrSelectionStart("\u202D"); break;
                    case "SORTLE": insertCharctorTocontextEditrSelectionStart("\u202B"); break;
                    case "SORTLO": insertCharctorTocontextEditrSelectionStart("\u202E"); break;

                }
            }

        }

        private void SurroundWithSpecialCharCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (IsLV(e.Source))
                e.CanExecute = MLVIsSomethingSelected;

        }

        private void SurroundWithSpecialCharCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (IsLV(e.Source))
                SurroundMainListViewSelectedItemsWithSpecialCharacters(true);



        }

        private void SubtitleAlignMentCommand_CaneExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mainListView.IsSomethingSelected();
        }

        private void SubtitleAlignMentCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (IsLV(e.Source))
            {
                ChangeSelectedLinesAlignment(e.Parameter as string, true);
            }
        }



        #endregion

        private void KarokeEffectCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            KaroakeEffectWindow kew = new KaroakeEffectWindow(_CurrentSubtitle,mainListView.SelectedItems as List<Line>,"Karoake Effect ");
            kew.Show();
        }

        private void KaroekeEffectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
           e.CanExecute= mainListView.SelectedItems.Count > 0 ? true : false;
        }
    }
}
