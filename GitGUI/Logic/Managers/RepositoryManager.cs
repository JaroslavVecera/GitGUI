using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;
using System.IO;
using System.Windows;

namespace GitGUI.Logic
{
    class RepositoryManager
    {
        RepositoryModel _current;
        string _dirPath = "Repos";
        public List<RepositoryModel> Repositories { get; private set; } = new List<RepositoryModel>();
        public IEnumerable<string> RecentRepos
        {
            get
            {
                var sorted = new List<RepositoryModel>(Repositories);
                sorted.Sort(CompareRepositories);
                return sorted.Take(7).Select(m => m.RepositoryPath);
            }
        }

        public delegate void RepositoryOpenedEventHandler(RepositoryModel repo);
        public delegate void RepositoryClosedEventHandler(RepositoryModel repo);

        public event Action<RepositoryModel> Opened;
        public event Action<RepositoryModel> Closed;

        public int CompareRepositories(RepositoryModel a, RepositoryModel b)
        {
            if (a.LastUse < b.LastUse)
                return 1;
            if (a.LastUse == b.LastUse)
                return 0;
            return -1;
        }

        public RepositoryManager()
        {
            LoadRepositories();
        }

        public RepositoryValidation Create(string path)
        {
            var v = LibGitService.GetInstance().IsValidRepository(path);
            if (v == RepositoryValidation.Invalid)
            {
                LibGitService.GetInstance().OpenNewRepository(path);
                Open(path);
            }
            return v;
        }

        public bool OpenExisting(string path)
        {
            var v = LibGitService.GetInstance().IsValidRepository(path);
            if (v == RepositoryValidation.Valid)
                return OpenValid(path);
            else if (v == RepositoryValidation.Invalid)
                return PromptCreatingNew(path);
            else
                InfoBare();
            return false;
        }

        void InfoBare()
        {
            MessageBox.Show("Can't open bare repository", "", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        bool OpenValid(string path)
        {
            LibGitService.GetInstance().OpenRepository(path);
            Open(path);
            return true;
        }

        bool PromptCreatingNew(string path)
        {
            MessageBoxResult rslt = MessageBox.Show("Do you want to create new?", "There is no valid repository", MessageBoxButton.YesNo, MessageBoxImage.Error);
            if (rslt == MessageBoxResult.No)
                return false;
            LibGitService.GetInstance().OpenNewRepository(path);
            Open(path);
            return true;
        }

        public void CloseCurrent()
        {
            Close();
            LibGitService.GetInstance().CloseCurrentRepository();
        }

        void Open(string path)
        {
            Close();
            _current = AddRepository(path);
            Opened?.Invoke(_current);
        }

        void Close()
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
