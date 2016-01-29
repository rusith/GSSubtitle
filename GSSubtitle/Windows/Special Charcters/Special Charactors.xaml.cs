using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace GSSubtitle.Windows.Special_Charcters
{
    public partial class Special_Charactors : Window
    {
        public StringBuilder returncharactors;

        public Special_Charactors(StringBuilder stringBuilderToReturn)
        {
            InitializeComponent();
            returncharactors = stringBuilderToReturn;
            multipleCheckbox.IsChecked = Properties.Settings.Default.specialcharwindowmultiplecheckboxischecked;

        }

        private void SpecialCharButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (multipleCheckbox.IsChecked == true)
                display.Text = display.Text + ((SpecialCharButton)sender).Charactor;
            else
            {
                DialogResult = true;
                returncharactors.Clear().Append(((SpecialCharButton)sender).Charactor);
                Close();
            }
                


        }

        private void closeImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OkImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            returncharactors.Clear().Append(display.Text);
            DialogResult = true;
            Close();
        }

        private void root_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.specialcharwindowmultiplecheckboxischecked =(bool)multipleCheckbox.IsChecked;
        }

        private void root_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            root.DragMove();
        }
    }
}
