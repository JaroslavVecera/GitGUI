using LibGit2Sharp;
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
            SelectedCommitIndex = 0;
            RefreshItems();
        }

        new CommitViewerTabModel Model { get { return (CommitViewerTabModel)base.Model; } }
        public string Message { get { return Model.Message; } }
        public IEnumerable<string >Commits { get { return Model.Commit.Commit.Parents.Select(p => p.MessageShort); } }
        public int SelectedCommitIndex { set { Model.SelectedCommitIndex = value; } }
        public Commit SelectedCommit { get { return Model.SelectedCommit; } }
        public bool SeveralCommits { get { return Commits != null && Commits.Count() > 1; } }
        public List<ChangesTreeItem> Items { get; private set; }
        public bool AnyItems { get { return Items.Any(); } }
        ChangesTreeItem _selected;
        public ChangesTreeItem SelectedItem { private get { return _selected; } set { _selected = value; OnPropertyChanged("ChangesInfo"); } }
        public ChangesInfo ChangesInfo { get { if (SelectedItem != null) return SelectedItem.Info; else return null; } }

        void RefreshItems()
        {
            Items = new List<ChangesTreeItem>() { };
            ChangesTreeDirectoryItem root = new ChangesTreeDirectoryItem() { Name = "All" };
            var commitChanges = Model.CommitChanges;
            if (commitChanges == null)
                return;
            var r2 = commitChanges.Modified;
            var r3 = commitChanges.Deleted;
            var r = commitChanges.Added;
            var r4 = commitChanges.Renamed;
            r2.ToList().ForEach(change => root.InsertItem(change.Path, ChangesInfo.Modified(change.Path, Model.Commit.Commit, SelectedCommit), false));
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
