using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GitGUI.Logic
{
    class ActionButton
    {
        Button _gElement;
        public string Text { get; private set; } = "";
        public bool Active { get; set; } = true;
        public Button GElement
        {
            get { return _gElement; }
            set { _gElement = value; _gElement.Click += OnClicked; }
        }

        public event RoutedEventHandler Clicked;

        public ActionButton(string text)
        {
            Text = text;
        }

        void OnClicked(object sender, RoutedEventArgs e)
        {
            Clicked?.Invoke(this, e);
        }
    }
}
