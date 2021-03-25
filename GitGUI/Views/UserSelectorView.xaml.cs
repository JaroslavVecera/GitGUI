using GitGUI.Logic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
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
    /// Interakční logika pro UserSelectorView.xaml
    /// </summary>
    public partial class UserSelectorView : UserControl
    {
        public event RoutedEventHandler ChangedUser
        {
            add { AddHandler(ChangedUserEvent, value); }
            remove { RemoveHandler(ChangedUserEvent, value); }
        }

        public static readonly RoutedEvent ChangedUserEvent = EventManager.RegisterRoutedEvent(
            "ChangedUser", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UserSelectorView));

        public UserSelectorView()
        {
            InitializeComponent();
        }

        private static readonly DependencyPropertyKey CUBitmapPropertyKey
        = DependencyProperty.RegisterReadOnly(
            nameof(CUBitmap),
            typeof(BitmapImage), typeof(UserSelectorView),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty CUBitmapProperty
            = CUBitmapPropertyKey.DependencyProperty;

        public BitmapImage CUBitmap
        {
            get { return (BitmapImage)GetValue(CUBitmapProperty); }
            protected set { SetValue(CUBitmapPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey CUNamePropertyKey
        = DependencyProperty.RegisterReadOnly(
            nameof(CUName),
            typeof(string), typeof(UserSelectorView),
            new FrameworkPropertyMetadata("",
                FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty CUNameProperty
            = CUBitmapPropertyKey.DependencyProperty;

        public string CUName
        {
            get { return (string)GetValue(CUNameProperty); }
            protected set { SetValue(CUNamePropertyKey, value); }
        }

        public static readonly DependencyProperty UsersProperty =
            DependencyProperty.Register(
                "Users", typeof(ObservableCollection<Logic.User>), typeof(UserSelectorView),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnUsersChanged)));

        public ObservableCollection<Logic.User> Users
        {
            get { return (ObservableCollection<Logic.User>)GetValue(UsersProperty); }
            set { SetValue(UsersProperty, value); }
        }

        private static void OnUsersChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            UserSelectorView control = (UserSelectorView)obj;

            control.OnUsersChanged();
        }

        private void OnUsersChanged()
        {
            if (Users.Count > 0)
                SelectUser(User.Anonym);
        }

        private void DisplayPopup(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = true;
        }

        private void UserSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Logic.User newUser = e.AddedItems.Count > 0 ? e.AddedItems.Cast<Logic.User>().Single() : User.Anonym;
            SelectUser(newUser);
            popup.IsOpen = false;
            ChangedUserEventArgs args = new ChangedUserEventArgs(ChangedUserEvent, newUser);
            RaiseEvent(args);
        }

        void SelectUser(Logic.User user)
        {
            CUName = user?.Name;
            CUBitmap = user?.PictureCopy;
        }

        public void SelectUser(int index)
        {
            list.SelectedIndex = index;
            SelectUser(Users[index]);
        }
    }
}
