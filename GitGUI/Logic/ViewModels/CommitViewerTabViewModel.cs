using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    public class CommitViewerTabViewModel : TabViewModel
    {
        public CommitViewerTabViewModel(CommitViewerTabModel m) : base(m)
        {
            SubscribeModel(m);
            RefreshItems();
        }

        new CommitViewerTabModel Model { get { return (CommitViewerTabModel)base.Model; } }
        public string Message { get { return Model.Message; } }
        public List<ChangesTreeItem> Items { get; private set; }
        public bool AnyItems { get { return Items.Any(); } }
        ChangesTreeItem _selected;
        public ChangesTreeItem SelectedItem { private get { return _selected; } set { _selected = value; OnPropertyChanged("ChangesInfo"); } }
        public ChangesInfo ChangesInfo { get { if (SelectedItem != null) return SelectedItem.Info; else return null; } }

        void RefreshItems()
        {
            Items = new List<ChangesTreeItem>() { };
            ChangesTreeDirectoryItem root = new ChangesTreeDirectoryItem() { Name = "All" };
            var r2 = Model.CommitChanges.Modified;
            var r3 = Model.CommitChanges.Deleted;
            var r = Model.CommitChanges.Added;
            var r4 = Model.CommitChanges.Renamed;
            r2.ToList().ForEach(change => root.InsertItem(change.Path, ChangesInfo.Modified(change.Path, Model.Commit.Commit), false));
            r3.ToList().ForEach(change => root.InsertItem(change.Path, ChangesInfo.Deleted(change.Path, Model.Commit.Commit), false));
            r.ToList().ForEach(change => root.InsertItem(change.Path, ChangesInfo.Untracked(change.Path), false));
            r4.ToList().ForEach(change => { root.InsertItem(change.Path, ChangesInfo.Renamed(change.OldPath, change.Path), false); });
            if (root.Items.Any())
                Items.Add(root);
            OnPropertyChanged("Items");
            OnPropertyChanged("AnyItems");
        }

        bool AnyChecked(ChangesTreeFileItem f)
        {
            return f.IsChecked;
        }

        void SubscribeModel(CommitViewerTabModel model)
        {
            model.RepositoryStatusChanged += RefreshItems;
        }
    }
}
