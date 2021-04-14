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
    /// <summary>
    /// Interakční logika pro AuthentificationDialog.xaml
    /// </summary>
    public partial class AuthentificationDialog : WindowBase
    {
        public AuthentificationDialog()
        {
            InitializeComponent();
            OkButtonClick = new RelayCommand(() => DialogResult = true, () => Name.Any() && Password.Any());
            button.Command = OkButtonClick;
        }

        RelayCommand OkButtonClick { get; set; }

        public string MinorThreadGetName()
        {
            return Dispatcher.Invoke(() => { return Name; });
        }

        public string MinorThreadGetPassword()
        {
            return Dispatcher.Invoke(() => { return Password; });
        }

        new public string Name { get { return NameTextBox.Text; } }
        public string Password { get { return PasswordTextBox.Text; } }

        void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && OkButtonClick.CanExecute(null))
                OkButtonClick.Execute(null);
        }

        protected override void RefreshMaximizeRestoreButton() { }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            OkButtonClick.RaiseCanExecuteChanged();
        }
    }
}
