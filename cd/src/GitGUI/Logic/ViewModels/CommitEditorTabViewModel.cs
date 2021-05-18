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
    class CommitEditorTabViewModel : EditorTabViewModel
    {
        public CommitEditorTabViewModel(CommitEditorTabModel m) : base(m) { }

        new CommitEditorTabModel Model { get { return (CommitEditorTabModel)base.Model; } }

        override protected void RefreshItems()
        {
            Items = new List<ChangesTreeItem>() { };
            ChangesTreeDirectoryItem root = GetRootItem();
            HashSet<string> changed = new HashSet<string>();
            var repositoryStatus = Model.RepositoryStatus;
            var repositoryChanges = Model.RepositoryChanges;
            var untracked = repositoryStatus.Untracked;
            untracked.ToList().ForEach(statusEntry => root.InsertItem(statusEntry.FilePath, ChangesInfo.Untracked(statusEntry.FilePath), true));
            var added = repositoryStatus.Added;
            added.ToList().ForEach(statusEntry => root.InsertItem(statusEntry.FilePath, ChangesInfo.Untracked(statusEntry.FilePath), true));
            var modified = repositoryChanges.Modified;
            var deleted = repositoryChanges.Deleted;
            var renamed = repositoryChanges.Renamed;
            modified.ToList().ForEach(change => { root.InsertItem(change.Path, ChangesInfo.Modified(change.Path), true); changed.Add(change.Path); });
            deleted.ToList().ForEach(change => { root.InsertItem(change.Path, ChangesInfo.Deleted(change.Path), true); changed.Add(change.Path); });
            renamed.ToList().ForEach(change => { root.InsertItem(change.Path, ChangesInfo.Renamed(change.OldPath, change.Path), true); changed.Add(change.Path); });
            var removed = repositoryStatus.Removed;
            removed.Where(sc => !changed.Contains(sc.FilePath)).ToList().ForEach(sc => { root.InsertItem(sc.FilePath, ChangesInfo.Deleted(sc.FilePath), true); });
            var modified2 = repositoryStatus.Modified;
            modified2.Where(sc => !changed.Contains(sc.FilePath)).ToList().ForEach(sc => { root.InsertItem(sc.FilePath, ChangesInfo.Modified(sc.FilePath), true); });
            var renamed2 = repositoryStatus.RenamedInIndex;
            renamed2.Where(sc => !changed.Contains(sc.FilePath)).ToList().ForEach(sc => { root.InsertItem(sc.FilePath, ChangesInfo.Renamed(sc.HeadToIndexRenameDetails.OldFilePath, sc.FilePath), true); });
            if (root.Items.Any())
                Items.Add(root);
            NotifyRefresh();
        }
    }
}
