using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    abstract class EditorTabViewModel : TabViewModel
    {
        new EditorTabModel Model { get { return (EditorTabModel)base.Model; } }
        public RelayCommand Commit { get; set; }
        public string Message
        {
            get { return Model.Message; }
            set { Model.Message = value; Commit.RaiseCanExecuteChanged(); }
        }
        public List<ChangesTreeItem> Items { get; protected set; }
        public bool AnyItems { get { return Items.Any(); } }
        ChangesTreeItem _selected;
        public ChangesTreeItem SelectedItem { private get { return _selected; } set { _selected = value; OnPropertyChanged("ChangesInfo"); } }
        public ChangesInfo ChangesInfo { get { if (SelectedItem != null) return SelectedItem.Info; else return null; } }

        protected ChangesTreeDirectoryItem GetRootItem()
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

        protected abstract void RefreshItems();

        protected void NotifyRefresh()
        {
            OnPropertyChanged("Items");
            OnPropertyChanged("AnyItems");
        }

        public EditorTabViewModel(EditorTabModel model) : base(model)
        {
            SubscribeModel(model);
            Commit = new RelayCommand(
                () => { SetPaths(); Model.Commit(); },
                () => { return !string.IsNullOrWhiteSpace(Message) && AnyItems && AnyChecked((ChangesTreeDirectoryItem)Items.Single()); });
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

        protected void SetPaths()
        {
            Model.Staged = Items.Single().GetCheckedPaths("");
            Model.Unstaged = Items.Single().GetUncheckedPaths("");
        }

        void SubscribeModel(EditorTabModel model)
        {
            model.RepositoryStatusChanged += RefreshItems;
        }

    }
}
