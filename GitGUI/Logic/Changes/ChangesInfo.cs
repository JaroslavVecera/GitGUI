using LibGit2Sharp;

namespace GitGUI.Logic
{
    public abstract class ChangesInfo
    {
        public static ChangesInfo Modified(TreeEntryChanges entry)
        {
            return new ModifiedInfo(LibGitService.GetInstance().Diff(entry.Path));
        }

        public static ChangesInfo Modified(TreeEntryChanges entry, Commit c)
        {
            return new ModifiedInfo(LibGitService.GetInstance().Diff(entry.Path, c));
        }

        public static ChangesInfo Deleted(TreeEntryChanges entry)
        {
            return new DeletedInfo();
        }

        public static ChangesInfo Deleted(TreeEntryChanges entry, Commit c)
        {
            return Deleted(entry);
        }

        public static ChangesInfo Untracked(string path)
        {
            return new UntrackedInfo(LibGitService.GetInstance().Diff(path));
        }
    }
}