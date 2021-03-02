using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GitGUI.Logic
{
    class CommitEditorTabViewModel : TabViewModel
    {
        new CommitEditorTabModel Model { get { return (CommitEditorTabModel)base.Model; } }
        public RelayCommand Commit { get; set; }
        public string Message
        {
            get { return Model.Message; }
            set { Model.Message = value; Commit.RaiseCanExecuteChanged(); }
        }
        public List<ChangesTreeItem> Items { get; private set; }
        public bool AnyItems { get { return Items.Any(); } }
        ChangesTreeItem _selected;
        public ChangesTreeItem SelectedItem { private get { return _selected; } set { _selected = value; OnPropertyChanged("ChangesInfo"); } }
        public ChangesInfo ChangesInfo { get { if (SelectedItem != null) return SelectedItem.Info; else return null; } }

        ChangesTreeDirectoryItem GetRootItem()
        {
            ChangesTreeDirectoryItem root = new ChangesTreeDirectoryItem() { Name = "All" };
            root.Checked += () => Commit.RaiseCanExecuteChanged();
            root.Unchecked += () => Commit.RaiseCanExecuteChanged();
            root.SubItemCheckedChanged += () => Commit.RaiseCanExecuteChanged();
            return root;
        }

        bool HasFlag(FileStatus e, FileStatus flag)
        {
            return (e & flag) == flag;
        }

        void RefreshItems()
        {
            Items = new List<ChangesTreeItem>() { };
            ChangesTreeDirectoryItem root = GetRootItem();
            HashSet<string> changed = new HashSet<string>();
            var untracked = Model.RepositoryStatus.Untracked;
            untracked.ToList().ForEach(statusEntry => root.InsertItem(statusEntry.FilePath, ChangesInfo.Untracked(statusEntry.FilePath), false));
            var added = Model.RepositoryStatus.Added;
            added.ToList().ForEach(statusEntry => root.InsertItem(statusEntry.FilePath, ChangesInfo.Untracked(statusEntry.FilePath), true));
            var modified = Model.RepositoryChanges.Modified;
            var deleted = Model.RepositoryChanges.Deleted;
            modified.ToList().ForEach(change => { root.InsertItem(change.Path, ChangesInfo.Modified(change.Path), false); changed.Add(change.Path); });
            deleted.ToList().ForEach(change => { root.InsertItem(change.Path, ChangesInfo.Deleted(change.Path), false); changed.Add(change.Path); });
            var removed = Model.RepositoryStatus.Removed;
            removed.Where(sc => !changed.Contains(sc.FilePath)).ToList().ForEach(sc => { root.InsertItem(sc.FilePath, ChangesInfo.Deleted(sc.FilePath), true); });
            var modified2 = Model.RepositoryStatus.Modified;
            modified2.Where(sc => !changed.Contains(sc.FilePath)).ToList().ForEach(sc => { root.InsertItem(sc.FilePath, ChangesInfo.Modified(sc.FilePath), true); });
            if (root.Items.Any())
                Items.Add(root);
            NotifyRefresh();
        }

        void NotifyRefresh()
        { 
            OnPropertyChanged("Items");
            OnPropertyChanged("AnyItems");
        }

        public CommitEditorTabViewModel(CommitEditorTabModel model) : base(model)
        {
            SubscribeModel(model);
            Commit = new RelayCommand(
                () => { SetCheckedPaths(); Model.Commit(); },
                () => { return Message.Length > 0 && AnyChecked((ChangesTreeDirectoryItem)Items.Single()); });
            RefreshItems();
        }

        bool AnyChecked(ChangesTreeDirectoryItem dir)
        {
            return dir.IsChecked || dir.Items.Any(it =>
            {
                if (it is ChangesTreeDirectoryItem)
                    return AnyChecked((ChangesTreeDirectoryItem)it);
                else
                    return AnyChecked((ChangesTreeFileItem)it);
            });
        }

        bool AnyChecked(ChangesTreeFileItem f)
        {
            return f.IsChecked;
        }

        void SetCheckedPaths()
        {
            Model.Paths = Items.Single().GetCheckedPaths("");
        }

        void SubscribeModel(CommitEditorTabModel model)
        {
            model.RepositoryStatusChanged += RefreshItems;
        }
    }
}
