using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GSSubtitle.Windows.Special_Charcters
{
    /// <summary>
    /// Interaction logic for SpecialCharButton.xaml
    /// </summary>
    public partial class SpecialCharButton : UserControl
    {
 
        public static readonly DependencyProperty _BorderBrush = DependencyProperty.Register("Borderbrush", typeof(Brush), typeof(SpecialCharButton));

        public SpecialCharButton()
        {
            InitializeComponent();
            SetValue(_BorderBrush, Brushes.Gray);
        }

        public Brush Borderbrush
        {
            get { return (Brush)GetValue(_BorderBrush); }
            set { SetValue(_BorderBrush, value); }
        }
        public string Charactor
        {
            get { return Character.Text; }
            set { Character.Text = value; }
        }
        public double TextSize
        {
            get { return Character.FontSize; }
            set { Character.FontSize = value; }
        }
        public FontFamily TextFamily
        {
            get { return Character.FontFamily; }
            set { Character.FontFamily = value; }
        }

        public TextAlignment TextAlignment
        {
            get { return Character.TextAlignment; }
            set { Character.TextAlignment = value; }
        }

        public Brush BorderBruSh
        {
            get { return Border.BorderBrush; }
            set { Border.BorderBrush = value; }
        }

        public Brush TextColor
        {
            get { return Character.Foreground; }
            set { Character.Foreground = value; }
        }
        public CornerRadius Conerradius
        {
            get { return Border.CornerRadius; }
            set { Border.CornerRadius = value; }
        }

        public Thickness Borderthikness
        {
            get { return Border.BorderThickness; }
            set { Border.BorderThickness = value; }
        }
    }
}
