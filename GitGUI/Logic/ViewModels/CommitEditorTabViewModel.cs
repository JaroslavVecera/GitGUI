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
        CommitEditorTabModel Model { get; set; }
        public RelayCommand Commit { get; set; }
        public string Message
        {
            get { return Model.Message; }
            set { Model.Message = value; Commit.RaiseCanExecuteChanged(); }
        }
        public List<ChangesTreeItem> Items { get; private set; }
        public bool AnyItems { get { return Items.Any(); } }
        public string Header { get { return "Create commit"; } }
        ChangesTreeItem _selected;
        public ChangesTreeItem SelectedItem { private get { return _selected; } set { _selected = value; OnPropertyChanged("ChangesInfo"); } }
        public ChangesInfo ChangesInfo { get { if (SelectedItem != null) return SelectedItem.Info; else return null; } }

        void RefreshItems()
        {
            Items = new List<ChangesTreeItem>() { };
            ChangesTreeDirectoryItem root = new ChangesTreeDirectoryItem() { Name = "All" };
            root.Checked += () => Commit.RaiseCanExecuteChanged();
            root.Unchecked += () => Commit.RaiseCanExecuteChanged();
            root.SubItemCheckedChanged += () => Commit.RaiseCanExecuteChanged();
            var r = Model.RepositoryStatus.Untracked;
            var r2 = Model.RepositoryChanges.Modified;
            var r3 = Model.RepositoryChanges.Deleted;
            r.ToList().ForEach(statusEntry => root.InsertItem(statusEntry.FilePath, ChangesInfo.Untracked(statusEntry.FilePath)));
            r2.ToList().ForEach(change => root.InsertItem(change.Path, ChangesInfo.Choose(change)));
            r3.ToList().ForEach(change => root.InsertItem(change.Path, ChangesInfo.Choose(change)));
            if (root.Items.Any())
                Items.Add(root);
            OnPropertyChanged("Items");
            OnPropertyChanged("AnyItems");
        }

        public CommitEditorTabViewModel(CommitEditorTabModel model) : base(model)
        {
            Model = model;
            SubscribeModel(model);
            RefreshItems();
            Commit = new RelayCommand(
                () => { SetCheckedPaths(); Model.Commit(); },
                () => { return Message.Length > 0 && AnyChecked((ChangesTreeDirectoryItem)Items.Single()); });
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
