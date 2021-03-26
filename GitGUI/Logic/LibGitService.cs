﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using LibGit2Sharp;

namespace GitGUI.Logic
{
    public enum RepositoryValidation
    {
        Valid,
        Invalid,
        ValidBare
    }

    class LibGitService
    {
        string CheckoutedBranch { get; set; }
        public event Action BranchChanged;
        static LibGitService _instance;
        public event Action RepositoryChanged;
        Repository _repository;
        public Repository Repository { get { return _repository; }
            private set { _repository = value; } }
        public Branch Head { get { return Repository.Head; } }
        public TreeChanges CurrentChanges { get { return Repository.Diff.Compare<TreeChanges>(); } }
        public bool HasChanges {
            get
            {
                TreeChanges c = CurrentChanges;
                RepositoryStatus s = Status;
                return c.Added.Any() || c.Conflicted.Any() || c.Copied.Any() || c.Deleted.Any() || c.Modified.Any() || c.Renamed.Any() || c.TypeChanged.Any()
                    || s.IsDirty;
            }
        }
        public TreeChanges CommitChanges(Commit c)
        {
            if (!c.Parents.Any())
                return null;
            Commit parent = c.Parents.First();
            return CommitChanges(c, parent);
        }

        public TreeChanges CommitChanges(Commit c, Commit parent)
        {
            TreeChanges changes = Repository.Diff.Compare<TreeChanges>(parent.Tree, c.Tree);
            return changes;
        }
        public RepositoryStatus Status { get { return Repository.RetrieveStatus(); } }
        FileSystemWatcher Watcher { get; set; }
        public IQueryableCommitLog Commits { get { return Repository?.Commits; } }
        public BranchCollection Branches { get { return Repository?.Branches; } }
        public bool IsInConflictState { get { return CurrentChanges.Conflicted.Any(); } }
        public IEnumerable<Commit> AllCommits
        {
            get
            {
                List<Commit> res = new List<Commit>();
                foreach (Branch b in Repository?.Branches)
                    res = res.Union(Commits.QueryBy(new CommitFilter() { IncludeReachableFrom = b })).ToList();
                res.Sort((c1, c2) =>
                {
                    if (c1.Author.When > c2.Author.When)
                        return 1;
                    else if (c1.Author.When == c2.Author.When)
                        return 0;
                    else return -1;
                });
                return res;
            }
        }

        public void AbortMerge()
        {
            Repository.Reset(ResetMode.Hard);
        }

        private LibGitService() { }

        public PatchEntryChanges Diff(string path)
        {
            Patch p = Repository.Diff.Compare<Patch>(Repository.Head.Tip.Tree, DiffTargets.WorkingDirectory, new List<string>() { path });
            return p[path];
        }

        public ContentChanges Diff(string path, Commit c)
        {
            if (!c.Parents.Any())
                return null;
            Commit p = c.Parents.ToList().First();
            return Diff(path, c, p);
        }

        public ContentChanges Diff(string path, Commit c, Commit parent)
        {
            return Repository.Diff.Compare((Blob)parent[path].Target, (Blob)c[path].Target);
        }

        public RepositoryValidation IsValidRepository(string path)
        {
            if (!Repository.IsValid(path))
                return RepositoryValidation.Invalid;
            Repository bareTest = new Repository(path);
            bool res = !bareTest.Info.IsBare;
            bareTest.Dispose();
            return res ? RepositoryValidation.Valid : RepositoryValidation.ValidBare;
        }

        void DisableWatcher()
        {
            Watcher.Changed -= Fs;
            Watcher.Created -= Fs;
            Watcher.Deleted -= Fs;
            Watcher.Renamed -= Rs;
            Watcher = null;
        }

        void StartWatch(string path)
        {
            Watcher = new FileSystemWatcher()
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                             | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                Filter = "",
                Path = path,
                EnableRaisingEvents = true
            };
            SubscribeWatcherEvents(Watcher);
        }

        public Hashtable BranchCommits()
        {
            if (Repository == null)
                return null;
            Hashtable branchCommits = new Hashtable();
            HashSet<Commit> assigned = new HashSet<Commit>();
            Repository.Branches.ToList().ForEach(branch => branchCommits.Add(branch, new List<Commit>()));
            Repository.Branches.ToList().ForEach(branch =>
            {
                Commit current = branch.Tip;
                while (!assigned.Contains(current))
                {
                    assigned.Add(current);
                    ((List<Commit>)branchCommits[branch]).Add(current);
                    if (current.Parents.Count() == 0)
                        break;
                    current = current.Parents.First();
                }
            });
            return branchCommits;
        }

        public List<Tuple<Commit, int>> CommitRows()
        {
            DeployAlgorithm a = new DeployAlgorithm();
            var res = a.ComputeRows(AllCommits, Repository.Branches);
            res.Reverse();
            return res;
        }

        void Fs(object sender, FileSystemEventArgs e) =>
            Application.Current.Dispatcher.BeginInvoke((Action)(InvokeChange));

        void Rs(object sender, RenamedEventArgs e) =>
            Application.Current.Dispatcher.BeginInvoke((Action)(InvokeChange));

        void InvokeChange()
        {
            if (Repository == null)
                return;
            if (IsValidRepository(Repository.Info.Path) == RepositoryValidation.Valid)
            {
                RepositoryChanged?.Invoke();
                CheckBranch();
            }
            else
                Program.GetInstance().CloseCurrentRepository();
        }

        void SubscribeWatcherEvents(FileSystemWatcher watcher)
        { 
            watcher.Changed += Fs;
            watcher.Created += Fs;
            watcher.Deleted += Fs;
            watcher.Renamed += Rs;
        }

        public Repository OpenNewRepository(string path)
        {
            try
            {
                Repository.Init(path);
            }
            catch (LibGit2SharpException e)
            {
                throw new NotImplementedException();
            }
            return OpenRepository(path);
        }

        public void Add(IEnumerable<string> stagedFiles, IEnumerable<string> unstagedFiles)
        {
            if (unstagedFiles.Any())
                Commands.Unstage(Repository, unstagedFiles);
            Commands.Stage(Repository, stagedFiles);
        }

        public Repository OpenRepository(string path)
        {
            if (Repository != null)
                CloseRepository(Repository);
            Repository = new Repository(path);
            StartWatch(path);
            Program.GetInstance().StashingManager.SetRepository(Repository);
            RepositoryChanged.Invoke();
            CheckBranch();
            
            return Repository;
        }

        void CheckBranch()
        {
            string name = Repository?.Head.CanonicalName;
            if (CheckoutedBranch != name)
                BranchChanged?.Invoke();
            CheckoutedBranch = name;
        }

        Signature GetCurrentSignature()
        {
            return new Signature(Program.GetInstance().UserManager.Current.Identity, DateTimeOffset.Now);
        }

        public bool Merge(BranchLabelModel merged)
        {
            Signature s = GetCurrentSignature();
            MergeResult r = Repository.Merge(merged.Branch, s);
            return r.Status != MergeStatus.Conflicts;
        }

        public void Commit(BranchLabelModel l, string message)
        {
            Signature s = GetCurrentSignature();
            Commit c = Repository.Commit(message, s, s);
        }

        public Stash Stash(string message)
        {
            Signature s = GetCurrentSignature();
            return Repository.Stashes.Add(s, message);
        }

        public Stash Stash()
        {
            Signature s = GetCurrentSignature();
            return Repository.Stashes.Add(s);
        }

        public void Branch(GraphItemModel i, string name)
        {
            if (i != null)
                CreateBranch(name, (dynamic)i);
            else
                Repository.CreateBranch(name);
        }

        public void Checkout(GraphItemModel m)
        {
            if (m is BranchLabelModel)
                Commands.Checkout(Repository, ((BranchLabelModel)m).Branch);
            else
                Commands.Checkout(Repository, ((CommitNodeModel)m).Commit);
        }

        public void CloseCurrentRepository()
        {
            if (Repository != null)
                CloseRepository(Repository);
        }
        
        void CloseRepository(Repository r)
        {
            DisableWatcher();
            Repository.Dispose();
            Repository = null;
        }

        public bool IsValidRefName(string name)
        {
            string realName = "refs/heads/" + name;
            return Reference.IsValidName(realName) && !Repository.Refs.ToList().Any(b => b.CanonicalName == realName);
        }

        Branch CreateBranch(string name, BranchLabelModel l)
        {
            return Repository.CreateBranch(name, l.Branch.Tip);
        }

        Branch CreateBranch(string name, CommitNodeModel n)
        {
            return Repository.CreateBranch(name, n.Commit);
        }

        public static LibGitService GetInstance()
        {
            if (_instance == null)
                _instance = new LibGitService();
            return _instance;
        }
    }
}
