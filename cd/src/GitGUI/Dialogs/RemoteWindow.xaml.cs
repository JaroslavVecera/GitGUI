using System;
using System.Collections.Generic;
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

namespace GitGUI
{
    public enum RemoteWindowRole
    {
        Create,
        Edit
    }

    /// <summary>
    /// Interakční logika pro RemoteWindow.xaml
    /// </summary>
    public partial class RemoteWindow : WindowBase
    {
        bool IsNameValid { get { return Role == RemoteWindowRole.Edit || Logic.LibGitService.GetInstance().IsValidRefName(nameBox.Text); } }
        bool IsUrlValid { get { return url.Text.Any(); } }

        public static readonly DependencyProperty RoleProperty =
            DependencyProperty.Register("Role", typeof(RemoteWindowRole), typeof(RemoteWindow),
                new PropertyMetadata(RemoteWindowRole.Create, new PropertyChangedCallback(RoleChanged)));

        public RemoteWindowRole Role
        {
            get => (RemoteWindowRole)GetValue(RoleProperty);
            set => SetValue(RoleProperty, value);
        }

        static void RoleChanged(DependencyObject d, DependencyPropertyChangedEventArgs a)
        {
            RemoteWindow w = d as RemoteWindow;
            w.RoleChanged((RemoteWindowRole)a.NewValue);
        }

        void RoleChanged(RemoteWindowRole r)
        {
            nameBlock.Visibility = abort.Visibility = r == RemoteWindowRole.Create ? Visibility.Hidden : Visibility.Visible;
            nameBox.Visibility = nameCross.Visibility = namePipe.Visibility = r == RemoteWindowRole.Edit ? Visibility.Hidden : Visibility.Visible;

            submit.Content = r == RemoteWindowRole.Create ? "Create" : "Apply";
        }

        public static readonly DependencyProperty RemoteNameProperty =
            DependencyProperty.Register("RemoteName", typeof(string), typeof(RemoteWindow),
            new PropertyMetadata(""));

        public string RemoteName
        {
            get => (string)GetValue(RemoteNameProperty);
            set => SetValue(RemoteNameProperty, value);
        }

        public static readonly DependencyProperty UrlProperty =
            DependencyProperty.Register("Url", typeof(string), typeof(RemoteWindow),
            new PropertyMetadata(""));

        public string Url
        {
            get => (string)GetValue(UrlProperty);
            set => SetValue(UrlProperty, value);
        }

        public static readonly DependencyProperty UserNameProperty =
            DependencyProperty.Register("UserName", typeof(string), typeof(RemoteWindow),
                new PropertyMetadata(""));

        public string UserName
        {
            get => (string)GetValue(UserNameProperty);
            set => SetValue(UserNameProperty, value);
        }

        public static readonly DependencyProperty PasswordProperty =
           DependencyProperty.Register("Password", typeof(string), typeof(RemoteWindow),
           new PropertyMetadata(""));

        public string Password
        {
            get => (string)GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }

        public RemoteWindow()
        {
            InitializeComponent();
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInput();
        }

        void ValidateInput()
        {
            ValidateSubmitButton();
            ValidateName();
            ValidateUrl();
        }

        void ValidateSubmitButton()
        {
            bool isSubmitEnabled = IsNameValid && IsUrlValid;
            submit.IsEnabled = isSubmitEnabled;
        }

        void ValidateName()
        {
            if (Role == RemoteWindowRole.Edit)
                return;
            nameCross.Visibility = IsNameValid ? Visibility.Hidden : Visibility.Visible;
            namePipe.Visibility = IsNameValid ? Visibility.Visible : Visibility.Hidden;
        }

        void ValidateUrl()
        {
            urlCross.Visibility = IsUrlValid ? Visibility.Hidden : Visibility.Visible;
            urlPipe.Visibility = IsUrlValid ? Visibility.Visible : Visibility.Hidden;
        }

        void Submit(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        void Abort(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void RefreshMaximizeRestoreButton() { }
    }
}
