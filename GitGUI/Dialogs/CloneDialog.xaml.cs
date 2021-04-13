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
    /// Interakční logika pro CloneDialog.xaml
    /// </summary>
    public partial class CloneDialog : Window
    {
        public CloneDialog()
        {
            InitializeComponent();
            OkButtonClick = new RelayCommand(
                () => DialogResult = true);
            button.Command = OkButtonClick;
        }

        public string ResponseText
        {
            get { return ResponseTextBox.Text; }
            set { ResponseTextBox.Text = value; }
        }

        RelayCommand OkButtonClick { get; set; }
    }
}
