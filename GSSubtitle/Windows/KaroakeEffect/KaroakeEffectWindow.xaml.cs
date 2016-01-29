using GSSubtitle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SubLine = GSSubtitle.Models.SubtitleItems.Line;
using GSSubtitle.Tools.Extensions;
using System.Text.RegularExpressions;
using GSSubtitle.Tools;
using System.Collections;

namespace GSSubtitle.Windows.KaroakeEffect
{
    public partial class KaroakeEffectWindow : Window
    {
        List<SubLine> _lines;
        Subtitle _subtitle;
        List<Paragraph> animation;
        double keytime = 0;

        public KaroakeEffectWindow(Subtitle subtitle,System.Collections.IList lines,string title)
        {
            lines = lines as List<SubLine>;
            if (subtitle == null || lines == null||lines.Count<1) return;
            InitializeComponent();
            _subtitle = subtitle;
            Title = title;
            FlowDocument flowdocument = new FlowDocument();
            Paragraph para = new Paragraph();
            string str = _lines.OrderByDescending(x => x.Context.Length).ToList()[0].Context.Replace("</br>", Environment.NewLine);
            str = HtmlTools.RemoveHtmlTags(str);
            var run = new Run(str);

            run.Foreground = Brushes.White;
            para.Inlines.Add(run);
            flowdocument.Blocks.Add(para);
            preview.Document = flowdocument;
        }

        private void previewButton_Click(object sender, RoutedEventArgs e)
        {
            string context="";
            double duration=0;
            double numberofchars = 0;
            int speedfromui = Convert.ToInt32(speedslider.Value);
            animation = new List<Paragraph>();
            SubLine line = _lines.OrderByDescending(x => x.Context.Length).ToList()[0];
            context = HtmlTools.RemoveHtmlTags(line.Context.Replace("</br>", Environment.NewLine));
            numberofchars = Convert.ToDouble(context.Length);


            duration = line.Duration * 1000;
            double speed = speedfromui+50;
            double b = (speed) / 50d;
            keytime = (duration / numberofchars) /b;

            var color1 = Color1ColorPicker.SelectedColor.Value;
            var color2 = Color2ColorPicker.SelectedColor.Value;
            animation.Clear();
            for (int i = 0; i <=context.Length; i++)
            {
                Run c1 = new Run(context.Substring(0,i));
                c1.Foreground =new SolidColorBrush(color1);

                Run c2 = new Run(context.Substring(i, context.Length - i));
                c2.Foreground = new SolidColorBrush(color2);
                Paragraph para = new Paragraph();
                para.Inlines.Add(c1);
                para.Inlines.Add(c2);
                animation.Add(para);
            }
            RunPreveiw();
        }

        private void RunPreveiw()
        {
            Thread trh = new Thread(priveiwplayer);
            trh.Start();
        }


        private void priveiwplayer()
        {
            int kt = Convert.ToInt32(keytime);
            foreach (Paragraph paragraph in animation)
            {
                Thread.Sleep(kt);

                preview.Dispatcher.BeginInvoke((Action)(() => 
                {
                    preview.Document.Blocks.Clear();
                    preview.Document.Blocks.Add(paragraph);

                }));
                    
            }
                

            Thread.CurrentThread.Abort();

        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            int newlinecount = CountNewLines();
            if (newlinecount > 20)
            {
                var msgboxresult = MessageBox.Show(string.Format("{0} New Lines Will Added To Subtitle.\nAre You want To Continue?", newlinecount), "Confirm To Continue", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (msgboxresult != MessageBoxResult.OK) return;
                else
                {
                    foreach (SubLine line in _lines)
                    {
                        var currentindex = _subtitle.Lines.IndexOf(line);
                        foreach (char cha in line.Context)
                        {

                        }

                    }
                }
            }
            //
            foreach (SubLine line in _lines)
            {
                string context = line.Context;

            }
        }

        private int CountNewLines()
        {
            int count = 0;
            foreach (SubLine line in _lines)
                count += HtmlTools.RemoveHtmlTags(line.Context.Replace("</br>", "")).Length-1;
            return count;
            
        }
    }
}
