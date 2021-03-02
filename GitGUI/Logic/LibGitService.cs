using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LibGit2Sharp;

namespace GitGUI.Logic
{
    class LibGitService
    {
        static LibGitService _instance;
        public event Action RepositoryChanged;
        Repository _repository;
        Repository Repository { get { return _repository; } set { _repository = value; } }
        public Branch Head { get { return Repository.Head; } }
        public TreeChanges CurrentChanges { get { return Repository.Diff.Compare<TreeChanges>(); } }
        public TreeChanges CommitChanges(Commit c)
        {
            Tree commitTree = c.Tree;
            Tree parentCommitTree = c.Parents.First().Tree;
            TreeChanges changes = Repository.Diff.Compare<TreeChanges>(parentCommitTree, commitTree);
            return changes;
        }
        public RepositoryStatus Status { get { return Repository.RetrieveStatus(); } }
        FileSystemWatcher Watcher { get; set; }
        public IQueryableCommitLog Commits { get { return Repository?.Commits; } }
        public BranchCollection Branches { get { return Repository?.Branches; } }
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

        private LibGitService() { }

        public string Diff(string path)
        {
            return Repository.Diff.Compare<Patch>(Repository.Head.Tip.Tree, DiffTargets.WorkingDirectory);
        }

        public string Diff(string path, Commit c)
        {
            string d = "";
            c.Parents.ToList().ForEach(p =>
            {
                d += Repository.Diff.Compare<Patch>(p.Tree, c.Tree)[path].Patch;
            });
            return d;
        }

        public bool IsValidRepository(string path)
        {
            return LibGit2Sharp.Repository.IsValid(path);
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
            Application.Current.Dispatcher.BeginInvoke((Action)(() => RepositoryChanged?.Invoke()));

        void Rs(object sender, RenamedEventArgs e) =>
            Application.Current.Dispatcher.BeginInvoke((Action)(() => RepositoryChanged?.Invoke()));

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
            Commands.Unstage(Repository, unstagedFiles);
            Commands.Stage(Repository, stagedFiles);
        }

        public Repository OpenRepository(string path)
        {
            if (Repository != null)
                CloseRepository(Repository);
            Repository = new Repository(path);
            StartWatch(path);
            RepositoryChanged.Invoke();
            
            return Repository;
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

        public void Branch(GraphItemModel i, string name)
        {
            if (i != null)
                CreateBranch(name, (dynamic)i);
            else
                Repository.CreateBranch(name);
        }

        public void Checkout(BranchLabelModel b)
        {
            Commands.Checkout(Repository, b.Branch);
        }

        void CloseRepository(Repository r)
        {
            DisableWatcher();
            Repository.Dispose();
            Repository = null;
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
