﻿using System;
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
        public BranchLabelModel CurrentBranch { get; }
        public TreeChanges CurrentChanges { get { return Repository.Diff.Compare<TreeChanges>(); } }
        public RepositoryStatus Status { get { return Repository.RetrieveStatus(); } }
        FileSystemWatcher Watcher { get; set; }
        public IQueryableCommitLog Commits { get { return Repository?.Commits; } }

        private LibGitService() { }

        public string Diff(string path)
        {
            return Repository.Diff.Compare<Patch>(new List<string>() { path }, true);
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

        public void Add(IEnumerable<string> files)
        {
            Commands.Stage(Repository, files);
        }

        public Repository OpenRepository(string path)
        {
            if (Repository != null)
                CloseRepository(Repository);
            Repository = new Repository(path);
            StartWatch(path);
            return Repository;
        }

        public void Merge(BranchLabelModel merging, BranchLabelModel merged)
        {
            Signature s = new Signature(new Identity("anonym", "anonym@upol.cz"), DateTimeOffset.Now);
            MergeResult r = Repository.Merge(merging.Branch, s);
        }

        public void Commit(BranchLabelModel l, string message)
        {
            Signature s = new Signature(new Identity("anonym", "anonym@upol.cz"), DateTimeOffset.Now);
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
