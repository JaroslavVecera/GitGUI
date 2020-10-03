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
    class ActionButton : INotifyPropertyChanged
    {
        Button _gElement;
        public string Text { get; private set; }
        public bool Active { get; set; } = true;
        public Button GElement
        {
            get { return _gElement; }
            set { SetupGElement(value);  }
        }

        public event RoutedEventHandler Clicked;
        public event PropertyChangedEventHandler PropertyChanged;

        public ActionButton(string text)
        {
            Text = text;
        }

        void SetupGElement(Button b)
        {
            _gElement = b;
            _gElement.Click += OnClicked;
            _gElement.Content = Text;
        }

        void OnClicked(object sender, RoutedEventArgs e)
        {
            Clicked?.Invoke(this, e);
        }
    }
}
