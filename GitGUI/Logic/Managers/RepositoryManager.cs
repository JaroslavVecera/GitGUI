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
        RepositoryModel _current;
        public List<RepositoryModel> Repositories { get; private set; } = new List<RepositoryModel>();

        public delegate void RepositoryOpenedEventHandler(RepositoryModel repo);
        public delegate void RepositoryClosedEventHandler(RepositoryModel repo);
        
        public event Action<RepositoryModel> Opened;
        public event Action<RepositoryModel> Closed;

        void Create(string path)
        {
            LibGitService.GetInstance().OpenNewRepository(path);
            if (!Repositories.Exists(r => r.Path == path))
                AddModel(path);
            Open(new RepositoryModel(path));
        }

        void AddModel(string path)
        {
            RepositoryModel m = new RepositoryModel(path);
            Repositories.Add(m);
        }

        public void OpenExisting(RepositoryModel m)
        {
            LibGitService.GetInstance().OpenRepository(m.Path);
            Open(m);
        }

        void Open(RepositoryModel m)
        {
            if (_current != null)
                CloseCurrent();
            _current = m;
            Opened?.Invoke(_current);
        }

        void CloseCurrent()
        {
            _current.LastUse = DateTimeOffset.Now;
            Closed?.Invoke(_current);
        }
    }
}
