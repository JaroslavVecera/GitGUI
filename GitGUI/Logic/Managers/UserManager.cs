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
        string _dirPath = "Users";
        User _current;
        public User Current {
            get { return _current ?? User.Anonym; }
            set { _current = value; }
        }

        public ObservableCollection<User> KnownUsers { get; } = new ObservableCollection<User>();

        public UserManager()
        {
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
            if (w.ShowDialog() == true)
            {
                DeleteUser(u);
                AddUser(w.UserName, w.Email, w.Bitmap);
            }
        }

        public void DeleteUser(User u)
        {
            KnownUsers.Remove(u);
            u.Delete();
        }

        public void AddUser(string name, string email, Bitmap picture)
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
    }
}
