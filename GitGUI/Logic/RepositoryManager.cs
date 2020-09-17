using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace GitGUI.Logic
{
    class RepositoryManager
    {
        Repository _current;
        public List<Repository> Repositories { get; private set; } = new List<Repository>();

        public delegate void RepositoryOpenedEventHandler(LibGit2Sharp.Repository repo);
        public delegate void RepositoryClosedEventHandler(Repository repo);
        
        public event RepositoryOpenedEventHandler Opened;
        public event RepositoryClosedEventHandler Closed;

        public void Create(string path)
        {
            Create(path, DateTimeOffset.Now);
        }

        void Create(string path, DateTimeOffset t)
        {
            Repository r = new Repository() { Path = path, LastUse = t };
            Repositories.Add(r);
            Open(r);
        }

        void Open(Repository repo)
        {
            _current = repo;
            LibGit2Sharp.Repository r = new LibGit2Sharp.Repository(repo.Path);
            Opened?.Invoke(r);
        }

        void Close(Repository repo)
        {
            Closed?.Invoke(_current);
            if (_current != null)
                _current.LastUse = DateTimeOffset.Now;
        }
    }
}
