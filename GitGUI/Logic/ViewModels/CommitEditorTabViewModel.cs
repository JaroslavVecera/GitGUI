using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public string Header { get { return "Create commit"; } }
        void RefreshItems()
        {
            Items = new List<ChangesTreeItem>() { };
            ChangesTreeDirectoryItem root = new ChangesTreeDirectoryItem() { Name = "All" };
            root.Checked += () => Commit.RaiseCanExecuteChanged();
            root.Unchecked += () => Commit.RaiseCanExecuteChanged();
            root.SubItemCheckedChanged += () => Commit.RaiseCanExecuteChanged();
            var r = Model.RepositoryStatus.Untracked;
            r.ToList().ForEach(statusEntry => root.InsertItem(statusEntry.FilePath, statusEntry.State));
            Items.Add(root);
            OnPropertyChanged("Items");
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
