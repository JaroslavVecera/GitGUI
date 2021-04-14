using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GitGUI.Logic
{
    class RemoteManager
    {
        bool CanSelect { get; set; } = true;
        string _dirPath;
        Remote _selectedRemote;
        public Remote SelectedRemote { get { return _selectedRemote; } set { if (CanSelect)
                    _selectedRemote = value; } }
        public LibGit2Sharp.Remote SelectedRepositoryRemote { get { return CurrentRepositoryRemotes?.ToList().Find(r => r.Name == SelectedRemote.Name && r.Url == SelectedRemote.Url); } }
        LibGit2Sharp.RemoteCollection CurrentRepositoryRemotes { get { return LibGitNetworkService.GetInstance().Remotes; } }
        LibGit2Sharp.Repository CurrentRepository { get; set; }
        string CurrentRepositoryDirectory { get; set; }
        public ObservableCollection<Remote> CurrentRemotes { get; } = new ObservableCollection<Remote>();

        public RemoteManager(string dataFolder)
        {
            _dirPath = Path.Combine(dataFolder, "Remotes");
            LibGitService.GetInstance().RepositoryChanged += ChangeRemotes;
            LibGitService.GetInstance().BranchUpdated += AggressiveSelectRemote;
        }

        public void Reset()
        {
            SelectedRemote = null;
            ChangeRemotes();
            AggressiveSelectRemote();
        }

        void AggressiveSelectRemote()
        {
            if (CurrentRepository == null)
                return;
            Remote r = CurrentRemotes.ToList().Find(rep => rep.Name == CurrentRepository.Head.RemoteName);
            if (r != null)
                SelectedRemote = r;
            Program.GetInstance().Data.MainWindowModel.ForceNotify("SelectedRemote");
        }

        void SelectRemote()
        {
            if (CurrentRepository == null)
                return;
            Remote r = SelectedRemote;
            if (r == null)
                r = CurrentRemotes.ToList().FirstOrDefault(rem => rem.Name == SelectedRemote?.Name);
            if (r == null && CurrentRemotes.Any())
                r = CurrentRemotes.ToList().First();
            SelectedRemote = r;
            Program.GetInstance().Data.MainWindowModel.ForceNotify("SelectedRemote");
        }

        void ChangeRemotes()
        {
            CanSelect = false;
            CurrentRemotes.Clear();
            SetupRepository();
            if (CurrentRepository != null)
                SyncLoggedRemotes();
            CanSelect = true;
            SelectRemote();
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
            bool anyChanges = DeleteLoggedButNonExisting(repRemotes, remotes);
            remotes = remotes.Union(LogNew(repRemotes, remotes, anyChanges)).ToList();
            remotes.ForEach(r => CurrentRemotes.Add(r));
        }

        bool DeleteLoggedButNonExisting(List<LibGit2Sharp.Remote> repRemotes, List<Remote> remotes)
        {
            return remotes.RemoveAll(r => !repRemotes.Any(rr => rr.Name == r.Name && rr.Url == r.Url)) > 0;
        }

        List<Remote> LogNew(List<LibGit2Sharp.Remote> repRemotes, List<Remote> remotes, bool anyChanges)
        {
            List<Remote> newRemotes = repRemotes.Where(rr => !remotes.Any(r => r.Name == rr.Name && r.Url == rr.Url))
                                         .Select(rr => new Remote(rr.Name, rr.Url))
                                         .ToList();
            if (newRemotes.Any() || anyChanges)
                SaveRemotes(newRemotes.Union(remotes).ToList());
            return newRemotes;
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

        public void CreateRemote()
        {
            RemoteWindow w = new RemoteWindow();
            w.Owner = Application.Current.MainWindow;
            w.ShowDialog();
            if (w.DialogResult == true)
                CreateRemote(w.RemoteName, w.Url, w.UserName, w.Password);
        }

        void CreateRemote(string name, string url, string userName, string password)
        {
            if (CurrentRepositoryRemotes.Any(rr => rr.Name == name))
                MessageBox.Show(Application.Current.MainWindow, "Remote with name " + name + " already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                var r = new List<Remote>() { new Remote(name, url, userName, password) };
                SaveRemotes(r.Union(CurrentRemotes).ToList());
                LibGitNetworkService.GetInstance().AddRemote(name, url);
            }
        }

        public void RemoveRemote(Remote r)
        {
            LibGitNetworkService.GetInstance().RemoveRemote(r.Name);
        }

        public void EditRemote(Remote r)
        {
            RemoteWindow w = new RemoteWindow();
            w.Owner = Application.Current.MainWindow;
            w.Role = RemoteWindowRole.Edit;
            w.RemoteName = r.Name;
            w.Url = r.Url;
            w.UserName = r.UserName;
            w.Password = r.Password;
            w.ShowDialog();
            if (w.DialogResult == true)
                EditRemote(r, w.Url, w.UserName, w.Password);
        }

        void EditRemote(Remote r, string url, string userName, string password)
        {
            string oldUrl = r.Url;
            r.Update(url, userName, password);
            UpdateRemotes();
            if (oldUrl != url)
                LibGitNetworkService.GetInstance().UpdateRemote(r.Name, url);
        }

        void UpdateRemotes()
        {
            SaveRemotes(CurrentRemotes.ToList());
        }
    }
}
