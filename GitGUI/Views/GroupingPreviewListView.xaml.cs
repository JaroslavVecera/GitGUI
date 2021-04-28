using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interakční logika pro GroupingPreviewListView.xaml
    /// </summary>
    public partial class GroupingPreviewListView : UserControl
    {
        IEnumerable<NamedGroup> _groups;
        public Action SelectionAction { get; set; }

        public int? SelectedKey { get { return list.SelectedIndex > -1 ? ((GroupingPreviewListViewItem)list.SelectedItem).Key : (int?)null; } }

        public GroupingPreviewListView()
        {
            InitializeComponent();
        }

        public void SelectNext()
        {
            list.SelectedIndex++;
        }

        public void SelectPrevious()
        {
            if (list.SelectedIndex > 0)
                list.SelectedIndex--;
        }

        public IEnumerable<NamedGroup> Groups { get {return _groups; } set { _groups = value; UpdateContent(); } }

        void UpdateContent()
        {
            var items = BuildGroupingPreviewListViewItems();
            list.ItemsSource = items;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(list.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Header");
            view.GroupDescriptions.Add(groupDescription);
        }

        List<GroupingPreviewListViewItem> BuildGroupingPreviewListViewItems()
        {
            var res = new List<GroupingPreviewListViewItem>();
            foreach(var group in _groups)
            {
                foreach(var item in group.Items)
                {
                    res.Add(new GroupingPreviewListViewItem() { MatchingText = group.MatchingText, PreviewText = new StringReader(item.Item2).ReadLine(), Header = group.Header, Key = item.Item1 });
                }
            }
            return res;
        }

        public static readonly DependencyProperty MatchedBackgroundProperty = DependencyProperty.Register("MatchedBackground", typeof(Brush), typeof(GroupingPreviewListView),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Yellow)));

        public Brush MatchedBackground
        {
            get { return (Brush)GetValue(MatchedBackgroundProperty); }
            set { SetValue(MatchedBackgroundProperty, value); }
        }

        public static readonly DependencyProperty MatchedForegroundProperty = DependencyProperty.Register("MatchedForeground", typeof(Brush), typeof(GroupingPreviewListView));

        public Brush MatchedForeground
        {
            get { return (Brush)GetValue(MatchedForegroundProperty); }
            set { SetValue(MatchedForegroundProperty, value); }
        }

        class GroupingPreviewListViewItem
        {
            public int Key { get; set; }
            public string MatchingText { get; set; }
            public string PreviewText { get; set; }
            public string Header { get; set; }
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionAction?.Invoke();
        }
    }
}
