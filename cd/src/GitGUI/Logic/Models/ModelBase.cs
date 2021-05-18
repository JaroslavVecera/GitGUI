using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    public class ModelBase : PropertyChangedNotifier
    {

        public void ForceNotify(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }
    }
}
