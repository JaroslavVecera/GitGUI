using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GitGUI.Logic;

namespace GitGUI
{
    /// <summary>
    /// Interakční logika pro GroupingSuggestiveSearchBar.xaml
    /// </summary>
    public partial class GroupingSuggestiveSearchBar : UserControl
    {
        bool _moving = false;
        List<GraphItemModel> Items { get; } = new List<GraphItemModel>();

        void Search(string text)
        {
            Items.Clear();
            var found = Graph.GetInstance().Find(tb.Text);
            var bcmg = GroupCommitsByMessage(found.Item1, 0);
            var bbng = GroupBranches(found.Item2, Items.Count);
            var bcsg = GroupCommitsBySha(found.Item3, Items.Count);

            list.Groups = new List<NamedGroup>() { bcmg, bbng, bcsg };
        }

        NamedGroup GroupBranches(List<BranchLabelModel> models, int key)
        {
            int k = key;
            models.ForEach(b => Items.Add(b));
            var bbng = new NamedGroup() { Header = "By branch name", MatchingText = tb.Text };
            var bbngItems = models.Select(b => new Tuple<int, string>(k++, b.Name.Substring(tb.Text.Length)));
            key = k;
            bbng.Items = bbngItems;
            return bbng;
        }

        NamedGroup GroupCommitsByMessage(List<CommitNodeModel> models, int key)
        {
            int k = key;
            models.ForEach(b => Items.Add(b));
            var bcmg = new NamedGroup() { Header = "By commit message", MatchingText = tb.Text };
            var bcmgItems = models.Select(c => new Tuple<int, string>(k++, c.Message.Substring(tb.Text.Length)));
            key = k;
            bcmg.Items = bcmgItems;
            return bcmg;
        }

        NamedGroup GroupCommitsBySha(List<CommitNodeModel> models, int key)
        {
            int k = key;
            models.ForEach(b => Items.Add(b));
            var bcsg = new NamedGroup() { Header = "By commit SHA", MatchingText = tb.Text };
            var bcsgItems = models.Select(c => new Tuple<int, string>(k++, c.Sha.Substring(tb.Text.Length)));
            key = k;
            bcsg.Items = bcsgItems;
            return bcsg;
        }

        void Select(int key)
        {
            Graph.GetInstance().Aim(Items[key]);
        }

        public void RemoveFocus()
        {
            Keyboard.ClearFocus();
            popup.IsOpen = false;
            var isTabStop = tb.IsTabStop;
            tb.IsTabStop = false;
            tb.IsEnabled = false;
            tb.IsEnabled = true;
            tb.IsTabStop = isTabStop;
        }

        public GroupingSuggestiveSearchBar()
        {
            InitializeComponent();
            list.SelectionAction = Select;
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = false;
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            tb.Focus();
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (tb.Text != null && tb.Text.Any())
            {
                popup.IsOpen = true;
                Search(tb.Text);
            }
            else
                popup.IsOpen = false;
        }

        void Select()
        {
            if (_moving)
                return;
            var k = list.SelectedKey;
            if (k != null)
            {
                tb.Text = null;
                RemoveFocus();
                Select(k.Value);
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                RemoveFocus();
            else if (e.Key == Key.Down)
            {
                _moving = true;
                list.SelectNext();
                _moving = false;
            }
            else if (e.Key == Key.Up)
            {
                _moving = true;
                list.SelectPrevious();
                _moving = false;
            }
            else if (e.Key == Key.Enter)
            {
                Select();
            }
        }

        private void OnClosed(object sender, EventArgs e)
        {
            RemoveFocus();
        }
    }
}
