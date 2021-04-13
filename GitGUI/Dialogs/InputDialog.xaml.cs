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
    public partial class InputDialog : WindowBase
    {
        Func<string, bool> _validator;
        public Func<string, bool> Validator { get { return _validator; } set { _validator = value; SetCommand(); } }

        public InputDialog()
        {
            InitializeComponent();
            RefreshMaximizeRestoreButton();
        }

        public string Text { get; set; }

        void SetCommand()
        {
            OkButtonClick = new RelayCommand(
                () => DialogResult = true,
                () => Validator(ResponseText));
            button.Command = OkButtonClick;
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

        protected override void RefreshMaximizeRestoreButton() { }
    }
}
