using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSSubtitle.Models
{
    /// <summary>
    /// exposes basic binding power for models and view models
    /// </summary>
    public class BindingBase:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// call when any property changed this method will update bindings
        /// </summary>
        /// <param name="PropertyName"></param>
        public void OPC(string PropertyName)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }
    }
}
