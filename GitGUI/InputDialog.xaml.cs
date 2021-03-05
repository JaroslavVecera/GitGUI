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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GitGUI
{
    /// <summary>
    /// Interakční logika pro InputDialog.xaml
    /// </summary>
    public partial class InputDialog : Window
    {
        public InputDialog()
        {
            InitializeComponent();
            OkButtonClick = new RelayCommand(
                () => DialogResult = true,
                IsValidName);
            button.Command = OkButtonClick;
        }

        bool IsValidName()
        {
            return Logic.LibGitService.GetInstance().IsValidRefName(ResponseText);
        }

        public string ResponseText
        {
            get { return ResponseTextBox.Text; }
            set { ResponseTextBox.Text = value; }
        }

        RelayCommand OkButtonClick { get; set; }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            OkButtonClick.RaiseCanExecuteChanged();
        }
    }
}
