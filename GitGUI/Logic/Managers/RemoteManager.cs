using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    class RemoteManager
    {
        string _dirPath = "Remotes";
        public Remote SelectedRemote { get; set; }
        public LibGit2Sharp.Remote SelectedRepositoryRemote { get { return CurrentRepositoryRemotes?.ToList().Find(r => r.Name == SelectedRemote.Name && r.Url == SelectedRemote.Url); } }
        LibGit2Sharp.RemoteCollection CurrentRepositoryRemotes { get { return LibGitNetworkService.GetInstance().Remotes; } }
        LibGit2Sharp.Repository CurrentRepository { get; set; }
        string CurrentRepositoryDirectory { get; set; }
        public ObservableCollection<Remote> CurrentRemotes { get; } = new ObservableCollection<Remote>();

        public RemoteManager()
        {
            LibGitService.GetInstance().RepositoryChanged += ChangeRemotes;
            LibGitService.GetInstance().RepositoryChanged += SelectRemote;
            LibGitService.GetInstance().BranchUpdated += SelectRemote;
        }

        void SelectRemote()
        {
            if (CurrentRepository == null)
                return;
            Remote r = CurrentRemotes.ToList().Find(rep => rep.Name == CurrentRepository.Head.RemoteName);
            if (r == null && SelectedRemote == null && CurrentRemotes.Any())
                r = CurrentRemotes.ToList().First();
            if (r != null)
            {
                SelectedRemote = r;
                Program.GetInstance().Data.MainWindowModel.ForceNotify("SelectedRemote");
            }
        }

        void ChangeRemotes()
        {
            CurrentRemotes.Clear();
            SetupRepository();
            if (CurrentRepository != null)
                SyncLoggedRemotes();
        }

        void SetupRepository()
        {
            CurrentRepository = LibGitService.GetInstance().Repository;
            CurrentRepositoryDirectory = FindRepositoryLogPathIfExists();
            if (CurrentRepositoryDirectory == null)
                CurrentRepositoryDirectory = CreateRepositoryLog();
        }

        string CreateRepositoryLog()
        {
            string logPath = _dirPath + Path.DirectorySeparatorChar + FindNextRepositoryIndex();
            Directory.CreateDirectory(logPath);
            using (StreamWriter w = File.CreateText(logPath + Path.DirectorySeparatorChar + "Repo"))
            {
                w.WriteLine(CurrentRepository.Info.Path);
            }
            File.Create(logPath + Path.DirectorySeparatorChar + "Remotes").Dispose();
            return logPath;
        }

        string FindRepositoryLogPathIfExists()
        {
            Directory.CreateDirectory(_dirPath);
            foreach (string path in Directory.GetDirectories(_dirPath))
            {
                string logPath = path + Path.DirectorySeparatorChar;
                using (StreamReader r = new StreamReader(logPath + "Repo"))
                {
                    if (r.ReadLine() == CurrentRepository.Info.Path)
                        return logPath;
                }
            }
            return null;
        }

        string FindNextRepositoryIndex()
        {
            int name = 0;
            while (Directory.Exists(_dirPath + Path.DirectorySeparatorChar + name.ToString()))
                name++;
            return name.ToString();
        }

        void SyncLoggedRemotes()
        {
            var repRemotes = CurrentRepositoryRemotes.ToList();
            var remotes = GetRemotes();
            DeleteLoggedButNonExisting(repRemotes, remotes);
            remotes = remotes.Union(LogNew(repRemotes, remotes)).ToList();
            remotes.ForEach(r => CurrentRemotes.Add(r));
        }

        void DeleteLoggedButNonExisting(List<LibGit2Sharp.Remote> repRemotes, List<Remote> remotes)
        {
            List<Remote> nonExisting = remotes.Where(r => !repRemotes.Any(rr => rr.Name == r.Name && rr.Url == r.Url)).ToList();
            nonExisting.ForEach(DeleteRemote);
        }

        List<Remote> LogNew(List<LibGit2Sharp.Remote> repRemotes, List<Remote> remotes)
        {
            List<Remote> res = repRemotes.Where(rr => !remotes.Any(r => r.Name == rr.Name && r.Url == rr.Url))
                                         .Select(rr => new Remote(rr.Name, rr.Url))
                                         .ToList();
            SaveRemotes(res);
            return res;
        }

        void DeleteRemote(Remote r)
        {
            throw new NotImplementedException();
        }

        void SaveRemotes(List<Remote> remotes)
        {
            using (StreamWriter w = new StreamWriter(CurrentRepositoryDirectory + Path.DirectorySeparatorChar + "Remotes"))
            {
                remotes.ForEach(r =>
                {
                    w.WriteLine(r.Name);
                    w.WriteLine(r.Url);
                    w.WriteLine(r.UserName);
                    w.WriteLine(r.Password);
                });
            }
        }

        List<Remote> GetRemotes()
        {
            List<Remote> res = new List<Remote>();
            string fileName = CurrentRepositoryDirectory + Path.DirectorySeparatorChar + "Remotes";
            string[] lines = File.ReadAllLines(fileName);
            int i = 0, count = lines.Count();
            while (i < count)
            {
                string name = lines[i++];
                string url = lines[i++];
                string userName = lines[i++];
                string password = lines[i++];
                res.Add(new Remote(name, url, userName, password));
            }
            return res;
        }
    }
}
