using LibGit2Sharp;

namespace GitGUI.Logic
{
    public abstract class ChangesInfo
    {
        public static ChangesInfo Choose(TreeEntryChanges entry)
        {
            switch (entry.Status)
            {
                case ChangeKind.Modified:
                    return new ModifiedInfo(LibGitService.GetInstance().Diff(entry.Path));
                case ChangeKind.Deleted:
                    return new DeletedInfo();
            }
            return null;
        }

        public static ChangesInfo Untracked(string path)
        {
            return new UntrackedInfo(LibGitService.GetInstance().Diff(path));
        }
    }
}