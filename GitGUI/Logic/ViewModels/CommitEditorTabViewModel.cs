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
        public ICommand Commit { get; set; }
        public string Message { get { return Model.Message; } set { Model.Message = value; } }
        public List<ChangesTreeItem> Items { get; private set; }
        public string Header { get { return "Create commit"; } }
        void RefreshItems()
        {
            List<ChangesTreeItem> items = new List<ChangesTreeItem>() { new ChangesTreeDirectoryItem() { Name = "All" } };
            var r = Model.RepositoryStatus.Untracked;
            r.ToList().ForEach(statusEntry => items.Cast<ChangesTreeDirectoryItem>().Single().InsertItem(statusEntry.FilePath, statusEntry.State));
            Items = items;
            OnPropertyChanged("Items");
        }

        public CommitEditorTabViewModel(CommitEditorTabModel model) : base(model)
        {
            Model = model;
            SubscribeModel(model);
            RefreshItems();
            Commit = new RelayCommand(() => { SetCheckedPaths(); Model.Commit(); });
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
