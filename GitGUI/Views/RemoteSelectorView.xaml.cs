using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace GitGUI
{
    /// <summary>
    /// Interakční logika pro RemoteSelectorView.xaml
    /// </summary>
    public partial class RemoteSelectorView : UserControl
    {
        public string CRName
        {
            get { return (string)GetValue(CRNameProperty); }
            set { SetValue(CRNameProperty, value); }
        }

        public static readonly DependencyProperty CRNameProperty =
            DependencyProperty.Register("CRName", typeof(string), typeof(RemoteSelectorView));

        public Logic.Remote SelectedRemote
        {
            get { return (Logic.Remote)GetValue(SelectedRemoteProperty); }
            set { SetValue(SelectedRemoteProperty, value); }
        }

        public static readonly DependencyProperty SelectedRemoteProperty =
            DependencyProperty.Register("SelectedRemote", typeof(Logic.Remote), typeof(RemoteSelectorView), new FrameworkPropertyMetadata(OnSelectionChanged));

        public static readonly DependencyProperty RemotesProperty =
            DependencyProperty.Register(
                "Remotes", typeof(ObservableCollection<Logic.Remote>), typeof(RemoteSelectorView),
                new FrameworkPropertyMetadata(null));

        public ObservableCollection<Logic.Remote> Remotes
        {
            get { return (ObservableCollection<Logic.Remote>)GetValue(RemotesProperty); }
            set { SetValue(RemotesProperty, value); }
        }

        public RemoteSelectorView()
        {
            InitializeComponent();
        }

        static void OnSelectionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            RemoteSelectorView control = (RemoteSelectorView)obj;

            control.OnSelectionChanged();
        }

        void OnSelectionChanged()
        {
            if (SelectedRemote != null)
                CRName = SelectedRemote.Name;
            else
                CRName = "";
        }

        void OnSelectionChanged(Logic.Remote r)
        {
            if (r == null)
                SelectedRemote = null;
            else
            {
                SelectedRemote = r;
                CRName = SelectedRemote.Name;
            }
            popup.IsOpen = false;
        }

        private void RemoteSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var l = e.AddedItems.Cast<Logic.Remote>();
            if (l.Any())
                OnSelectionChanged(l.First());
            else
                OnSelectionChanged(null);
        }

        private void DisplayPopup(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = true;
        }
    }
}
