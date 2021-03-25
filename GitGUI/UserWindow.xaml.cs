using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
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
    public enum UserWindowRole
    {
        Create,
        Edit
    }

    /// <summary>
    /// Interakční logika pro UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        bool PictureChoosed { get; set; }
        bool IsNameValid { get { return name.Text.Any(); } }
        bool IsEmailValid { get { return email.Text.Any() && new EmailAddressAttribute().IsValid(email.Text); } }
        Bitmap _bitmap;

        public Bitmap Bitmap
        {
            get { return _bitmap; }
            set { _bitmap = value; if (value != null) Open(BitmapToBitmapImage(value)); }
        }

        public static readonly DependencyProperty RoleProperty =
            DependencyProperty.Register("Role", typeof(UserWindowRole), typeof(UserWindow),
                new PropertyMetadata(UserWindowRole.Create, new PropertyChangedCallback(RoleChanged)));

        public UserWindowRole Role
        {
            get => (UserWindowRole)GetValue(RoleProperty);
            set => SetValue(RoleProperty, value);
        }

        public static readonly DependencyProperty UserNameProperty =
            DependencyProperty.Register("UserName", typeof(string), typeof(UserWindow),
                new PropertyMetadata("", new PropertyChangedCallback(InputChanged)));

        public string UserName
        {
            get => (string)GetValue(UserNameProperty);
            set => SetValue(UserNameProperty, value);
        }

        public static readonly DependencyProperty EmailProperty =
            DependencyProperty.Register("Email", typeof(string), typeof(UserWindow),
                new PropertyMetadata("", new PropertyChangedCallback(InputChanged)));

        public string Email
        {
            get => (string)GetValue(EmailProperty);
            set => SetValue(EmailProperty, value);
        }

        static void RoleChanged(DependencyObject d, DependencyPropertyChangedEventArgs a)
        {
            UserWindow w = d as UserWindow;
            w.RoleChanged((UserWindowRole)a.NewValue);
        }

        void RoleChanged(UserWindowRole r)
        {
            abort.Visibility = r == UserWindowRole.Create ? Visibility.Collapsed : Visibility.Visible;
            submit.Content = r == UserWindowRole.Create ? "Create" : "Apply";
        }

        static void InputChanged(DependencyObject d, DependencyPropertyChangedEventArgs a)
        {
            UserWindow w = d as UserWindow;
            w.ValidateInput();
        }

        public UserWindow()
        {
            InitializeComponent();
            Role = UserWindowRole.Edit;
        }

        void Open(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openfiledialog = new OpenFileDialog();

            openfiledialog.Title = "Open Image";
            openfiledialog.Filter = "Image File|*.bmp; *.gif; *.jpg; *.jpeg; *.png;";
            if (openfiledialog.ShowDialog() == true)
            {
                Bitmap = new Bitmap(openfiledialog.FileName);
                Open(BitmapToBitmapImage(Bitmap));
            }
        }

        void Open(BitmapImage bmp)
        {
            image.Source = bmp;
            PictureChoosed = true;
            ValidateInput();
        }

        BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BitmapImage res = new BitmapImage();
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                stream.Position = 0;
                res.BeginInit();
                res.StreamSource = stream;
                res.CacheOption = BitmapCacheOption.OnLoad;
                res.EndInit();
                return res;
            }
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInput();
        }

        void ValidateInput()
        {
            ValidateSubmitButton();
            ValidateRemoveButton();
            ValidateName();
            ValidateEmail();
        }

        void ValidateRemoveButton()
        {
            remove.IsEnabled = PictureChoosed;
        }

        void ValidateSubmitButton()
        {
            bool isSubmitEnabled = IsNameValid && IsEmailValid;
            submit.IsEnabled = isSubmitEnabled;
        }

        void ValidateName()
        {
            nameCross.Visibility = IsNameValid ? Visibility.Hidden : Visibility.Visible;
            namePipe.Visibility = IsNameValid ? Visibility.Visible : Visibility.Hidden;
        }

        void ValidateEmail()
        {
            emailCross.Visibility = IsEmailValid ? Visibility.Hidden : Visibility.Visible;
            emailPipe.Visibility = IsEmailValid ? Visibility.Visible : Visibility.Hidden;
        }

        private void Remove(object sender, RoutedEventArgs e)
        {
            image.Source = null;
            PictureChoosed = false;
            ValidateInput();
        }

        private void Submit(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Abort(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
