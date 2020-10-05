using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GitGUI.Logic
{
    class ActionButtonModel : ModelBase
    {
        public string Text { get; set; }
        public bool Active { get; set; } = true;

        public event Action Clicked;

        void OnClicked(object sender, RoutedEventArgs e)
        {
            Clicked?.Invoke();
        }
    }
}
