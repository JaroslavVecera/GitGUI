using LibGit2Sharp;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace GitGUI.Logic
{
    public class UserManager
    {
        public event Action UsersChanged;
        string _dirPath;
        User _current;
        public User Current {
            get { return _current ?? User.Anonym; }
            set { _current = value; }
        }

        public Signature CurrentSignature
        {
            get { return new Signature(Current.Identity, DateTimeOffset.Now); }
        }

        public ObservableCollection<User> KnownUsers { get; } = new ObservableCollection<User>();

        public UserManager(string dataFolder)
        {
            _dirPath = Path.Combine(dataFolder, "Users");
            InitializeKnownUsers();
        }

        public void ChangeUser(User u)
        {
            Current = u;
        }

        public void InitializeKnownUsers()
        {
            Directory.CreateDirectory(_dirPath);
            foreach (string path in Directory.GetDirectories(_dirPath))
            {
                var user = new User(path);
                KnownUsers.Add(user);
            }
            if (!KnownUsers.Any(u => u.Email == "-" && u.Name == "Anonym"))
                KnownUsers.Add(User.Anonym);
        }

        public BitmapImage FindUserPictureByIdentity(LibGit2Sharp.Identity i)
        {
            User user = KnownUsers.ToList().Find(u => u.HasIdentity(i));
            if (user == null)
                return null;
            else
                return user.PictureCopy;
        }

        public void CreateNewUser()
        {
            var w = new UserWindow();
            if (w.ShowDialog() == true)
            {
                AddUser(w.UserName, w.Email, w.Bitmap);
            }
        }

        public void EditUser(User u)
        {
            var w = new UserWindow();
            w.Role = UserWindowRole.Edit;
            w.UserName = u.Name;
            w.Email = u.Email;
            w.Bitmap = u.BitmapCopy;
            w.ChangedBitmap = false;
            if (w.ShowDialog() == true)
            {
                if (!w.ChangedBitmap && w.Bitmap != null)
                    w.Bitmap.Dispose();
                DoDeleteUser(u);
                DoAddUser(w.UserName, w.Email, w.Bitmap);
            }
            Graph.GetInstance().DeployGraph();
        }

        public void DeleteUser(User u)
        {
            DoDeleteUser(u);
            Graph.GetInstance().DeployGraph();
        }

        void DoDeleteUser(User u)
        { 
            KnownUsers.Remove(u);
            u.Delete();
        }

        public void AddUser(string name, string email, Bitmap picture)
        {
            DoAddUser(name, email, picture);
            Graph.GetInstance().DeployGraph();
        }

        void DoAddUser(string name, string email, Bitmap picture)
        { 
            if (KnownUsers.ToList().Any(u => u.HasIdentity(new LibGit2Sharp.Identity(name,email))))
                return;
            KnownUsers.Add(new User(FindNextName(), name, email, picture));
            UsersChanged?.Invoke();
        }

        string FindNextName()
        {
            int name = 0;
            while (Directory.Exists(_dirPath + Path.DirectorySeparatorChar + name.ToString()))
                name++;
            return  _dirPath + Path.DirectorySeparatorChar + name.ToString();
        }

        public void ShareDatabase()
        {
            var dialog = new VistaFolderBrowserDialog();
            dialog.Description = "Select directory to copy user database";
            dialog.UseDescriptionForTitle = true;
            var ans = dialog.ShowDialog();
            if (ans != true)
                return;
            ShareDatabase(dialog.SelectedPath);
        }

        void ShareDatabase(string path)
        {
            string target = Path.Combine(path, "Users");
            Directory.CreateDirectory(target);
            CopyUsersDir(new DirectoryInfo(_dirPath), new DirectoryInfo(target));
        }

        void CopyUsersDir(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);
            foreach (DirectoryInfo s in source.GetDirectories())
            {
                DirectoryInfo ts = target.CreateSubdirectory(s.Name);
                CopyUser(s, ts);
            }
        }

        void CopyUser(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (FileInfo i in source.GetFiles())
                i.CopyTo(Path.Combine(target.FullName, i.Name), true);
        }

        public void UpdateDatabase()
        {
            var dialog = new VistaFolderBrowserDialog();
            dialog.Description = "Select user database to update from";
            dialog.UseDescriptionForTitle = true;
            var ans = dialog.ShowDialog();
            if (ans != true)
                return;
            try
            {
                UpdateDatabase(dialog.SelectedPath);
            }
            catch(Exception e)
            {
                MessageBox.Show("Corrupted user database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void UpdateDatabase(string path)
        {
            List<User> newUsers = new List<User>();
            foreach (string d in Directory.GetDirectories(path))
            {
                var user = new User(d);
                if (!KnownUsers.Any(u => u.Email == user.Email && u.Name == user.Name))
                    newUsers.Add(user);
            }
            newUsers.ForEach(u =>
            {
                AddUser(u.Name, u.Email, u.BitmapCopy);
            });
        }
    }
}
