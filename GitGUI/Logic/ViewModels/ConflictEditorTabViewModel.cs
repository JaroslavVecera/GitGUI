using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    class ConflictEditorTabViewModel : EditorTabViewModel
    {
        public RelayCommand Abort { get; set; }

        new ConflictEditorTabModel Model { get { return (ConflictEditorTabModel)base.Model; } }

        public ConflictEditorTabViewModel(EditorTabModel m) : base(m)
        {
            Abort = new RelayCommand(() => { Model.Abort(); });
        }

        override protected void RefreshItems()
        {
            Items = new List<ChangesTreeItem>() { };
            ChangesTreeDirectoryItem root = GetRootItem();
            HashSet<string> changed = new HashSet<string>();
            var untracked = Model.RepositoryStatus.Untracked;
            untracked.ToList().ForEach(statusEntry => root.InsertItem(statusEntry.FilePath, ChangesInfo.Untracked(statusEntry.FilePath), true));
            var added = Model.RepositoryStatus.Added;
            added.ToList().ForEach(statusEntry => root.InsertItem(statusEntry.FilePath, ChangesInfo.Untracked(statusEntry.FilePath), true));
            var modified = Model.RepositoryChanges.Modified;
            var deleted = Model.RepositoryChanges.Deleted;
            var renamed = Model.RepositoryChanges.Renamed;
            var conflicted = Model.RepositoryChanges.Conflicted;
            modified.ToList().ForEach(change => { root.InsertItem(change.Path, ChangesInfo.Modified(change.Path), true); changed.Add(change.Path); });
            deleted.ToList().ForEach(change => { root.InsertItem(change.Path, ChangesInfo.Deleted(change.Path), true); changed.Add(change.Path); });
            renamed.ToList().ForEach(change => { root.InsertItem(change.Path, ChangesInfo.Renamed(change.OldPath, change.Path), true); changed.Add(change.Path); });
            conflicted.ToList().ForEach(change => { root.InsertStaticItem(change.Path, ChangesInfo.Conflict(change.Path), true); changed.Add(change.Path); });
            var removed = Model.RepositoryStatus.Removed;
            removed.Where(sc => !changed.Contains(sc.FilePath)).ToList().ForEach(sc => { root.InsertItem(sc.FilePath, ChangesInfo.Deleted(sc.FilePath), true); });
            var modified2 = Model.RepositoryStatus.Modified;
            modified2.Where(sc => !changed.Contains(sc.FilePath)).ToList().ForEach(sc => { root.InsertItem(sc.FilePath, ChangesInfo.Modified(sc.FilePath), true); });
            var renamed2 = Model.RepositoryStatus.RenamedInIndex;
            renamed2.Where(sc => !changed.Contains(sc.FilePath)).ToList().ForEach(sc => { root.InsertItem(sc.FilePath, ChangesInfo.Renamed(sc.HeadToIndexRenameDetails.OldFilePath, sc.FilePath), true); });
            if (root.Items.Any())
                Items.Add(root);
            NotifyRefresh();
        }
    }
}
