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
    /// Interakční logika pro StashMenuView.xaml
    /// </summary>
    public partial class StashMenuView : UserControl
    {
        int _level = 0;
        public StashMenuView()
        {
            InitializeComponent();
        }

        public IEnumerable<Tuple<string, string>> Stashes
        {
            get { return (IEnumerable<Tuple<string, string>>)GetValue(StashesProperty); }
            set { SetValue(StashesProperty, value); }
        }

        public static readonly DependencyProperty StashesProperty =
            DependencyProperty.Register("Stashes", typeof(IEnumerable<Tuple<string, string>>), typeof(StashMenuView), new PropertyMetadata(OnStashesChanged));

        static void OnStashesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StashMenuView sh = (StashMenuView)d;
            if (sh._level > 0)
                return;
            sh._level++;
                var c = sh.DataContext;
            sh.DataContext = null;
            sh.DataContext = c;
            sh._level--;
        }
    }
}
