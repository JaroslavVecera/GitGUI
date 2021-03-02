﻿using LibGit2Sharp;

namespace GitGUI.Logic
{
    public abstract class ChangesInfo
    {
        public static ChangesInfo Renamed(string oldPath, string newPath)
        {
            return new RenamedInfo();
        }

        public static ChangesInfo Modified(string path)
        {
            return new ModifiedInfo(LibGitService.GetInstance().Diff(path));
        }

        public static ChangesInfo Modified(string path, Commit c)
        {
            return new ModifiedInfo(LibGitService.GetInstance().Diff(path, c));
        }

        public static ChangesInfo Deleted(string path)
        {
            return new DeletedInfo();
        }

        public static ChangesInfo Deleted(string path, Commit c)
        {
            return Deleted(path);
        }

        public static ChangesInfo Untracked(string path)
        {
            return new UntrackedInfo(LibGitService.GetInstance().Diff(path));
        }
    }
}