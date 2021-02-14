using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;
using System.IO;

namespace GitGUI.Logic
{
    class RepositoryManager
    {
        RepositoryModel _current;
        string _dirPath = "Repos";
        public List<RepositoryModel> Repositories { get; private set; } = new List<RepositoryModel>();

        public delegate void RepositoryOpenedEventHandler(RepositoryModel repo);
        public delegate void RepositoryClosedEventHandler(RepositoryModel repo);

        public event Action<RepositoryModel> Opened;
        public event Action<RepositoryModel> Closed;

        public RepositoryManager()
        {
            LoadRepositories();
        }

        public void Create(string path)
        {
            if (LibGitService.GetInstance().IsValidRepository(path))
                throw new NotImplementedException("Already exists");
            LibGitService.GetInstance().OpenNewRepository(path);
            Open(path);
        }

        public void OpenExisting(string path)
        {
            if (!LibGitService.GetInstance().IsValidRepository(path))
                throw new NotImplementedException("To do: Show that there is no valid repository on the path.");
            LibGitService.GetInstance().OpenRepository(path);
            Open(path);
        }

        void Open(string path)
        {
            CloseCurrent();
            _current = AddRepository(path);
            Opened?.Invoke(_current);
            Graph.GetInstance().DeployGraph();
        }

        public void CloseCurrent()
        {
            if (_current == null)
                return;
            _current.LastUse = DateTimeOffset.Now;
            Closed?.Invoke(_current);
        }

        void LoadRepositories()
        {
            Directory.CreateDirectory(_dirPath);
            foreach (string path in Directory.GetFiles(_dirPath))
            {
                var repo = new RepositoryModel(path);
                repo.Load();
                Repositories.Add(repo);
            }
        }

        RepositoryModel AddRepository(string repositoryPath)
        {
            var repo = Repositories.Find(r => r.RepositoryPath == repositoryPath);
            if (repo != null)
                return repo;
            string name = _dirPath + Path.DirectorySeparatorChar + FindNextName();
            RepositoryModel newRepoModel = new RepositoryModel(name)
            {
                RepositoryPath = repositoryPath,
                LastUse = DateTime.Now
            };
            Repositories.Add(newRepoModel);
            return newRepoModel;
        }

        string FindNextName()
        {
            int name = 0;
            while (File.Exists(_dirPath + Path.DirectorySeparatorChar + name.ToString()))
                name++;
            return name.ToString();
        }
    }
}
