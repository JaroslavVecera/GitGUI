using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    public class UserManager
    {
        string _dirPath = "Users";
        User _current;
        public User Current {
            get { return _current; }
            set { _current = value ?? User.Anonym; }
        }

        List<User> KnownUsers { get; } = new List<User>();

        public UserManager()
        {
            InitializeKnownUsers();
        }

        public void InitializeKnownUsers()
        {
            Directory.CreateDirectory(_dirPath);
            foreach (string path in Directory.GetDirectories(_dirPath))
            {
                var user = new User(path);
                KnownUsers.Add(user);
            }
        }

        public string FindUserPicturePathByIdentity(LibGit2Sharp.Identity i)
        {
            User user = KnownUsers.Find(u => u.HasIdentity(i));
            if (user == null)
                return null;
            else
                return user.PicturePath;
        }

        public void AddUser(string name, string email, Bitmap picture)
        {
            KnownUsers.Add(new User(FindNextName(), name, email, picture));
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
