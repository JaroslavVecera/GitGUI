using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GitGUI.Logic
{
    class ActionPanel
    {
        public StackPanel GElement
        {
            get { return ((MainWindow)Application.Current.MainWindow).ActionPanel; }
        }
        public ObservableCollection<ActionButton> Actions { get; } = new ObservableCollection<ActionButton>();

        public ActionPanel()
        {
            Actions.CollectionChanged += OnCollectionChanged;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach(ActionButton b in e.NewItems)
            {
                NewButton(b);
            }
        }

        void OnButtonClick(object sender, RoutedEventArgs e)
        {

        }

        void NewButton(ActionButton b)
        {
            Button gb = new Button();
            GElement.Children.Add(gb);
            b.GElement = gb;
        }
    }
}
