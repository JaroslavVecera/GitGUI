using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using LibGit2Sharp;

namespace GitGUI.Logic
{
    class UserManager
    {
        User _current;
        List<User> Users = new List<User>();
        public User Current
        {
            get { return _current; }
            set
            {
                if (!Users.Contains(value))
                    throw new InvalidOperationException("Cannot set User that is not added as current.");
                _current = value;
            }
        }

        public delegate void HandleUserEventHandler(User u);

        public event HandleUserEventHandler RemovedUser;
        public event HandleUserEventHandler AddedUser;

        public void Add(User u)
        {
            if (Users.Contains(u))
                throw new InvalidOperationException(
                    "User manager already contains user " + u.Name + " with email " + u.Email + ".");
            Users.Add(u);
            AddedUser?.Invoke(u);
        }

        public void Remove(User u)
        {
            Users.Remove(u);
            RemovedUser?.Invoke(u);
        }

        public string FindImagePath(Signature s)
        {
            User potencialUser = new User() { Name = s.Name, Email = s.Email };
            User match = Users.Find(u => u == potencialUser);
            if (match == null)
                return null;
            return match.ImagePath;
        }

        void ChangeProperty(User u, Action<User> a)
        {
            RemovedUser?.Invoke(u);
            a(u);
            AddedUser?.Invoke(u);
        }

        public void ChangeName(User u, string name)
        {
            ChangeProperty(u, x => x.Name = name);
        }

        public void ChangeEmail(User u, string email)
        {
            ChangeProperty(u, x => x.Email = email);
        }

        public void ChangeImagePath(User u, string path)
        {
            ChangeProperty(u, x => x.ImagePath = path);
        }
    }
}
