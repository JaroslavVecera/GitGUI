﻿using System;
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
    public class ActionButtonModel : ModelBase
    {
        string _text;
        public string Text
        {
            get { return _text; }
            set { _text = value; OnPropertyChanged(); }
        }
        bool _active = true;
        public bool Active { get { return _active; } set { _active = value; OnPropertyChanged(); } }

        public event Action Clicked;

        public void OnClicked(object sender, RoutedEventArgs e)
        {
            Clicked?.Invoke();
        }
    }
}
