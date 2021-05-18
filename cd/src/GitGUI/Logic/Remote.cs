using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    public class Remote : PropertyChangedNotifier
    {
        public string Name { get; private set; }
        public string Url { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public RelayCommand OnDelete { get; private set; }
        public RelayCommand OnEdit { get; private set; }

        public Remote()
        {
            InitializeCommands();
        }

        public Remote(string name, string url) : this(name, url, "", "") { }

        public Remote(string name, string url, string userName, string password)
        {
            Name = name;
            Update(url, userName, password);
            InitializeCommands();
        }

        void InitializeCommands()
        {
            OnDelete = new RelayCommand(() => LibGitNetworkService.GetInstance().DeleteRemote(Name));
            OnEdit = new RelayCommand(() => Program.GetInstance().RemoteManager.EditRemote(this));
        }

        public void Update(string url, string userName, string password)
        {
            Url = url;
            UserName = userName;
            Password = password;
            NotifyChange();
        }

        void NotifyChange()
        {
            OnPropertyChanged("UserName");
            OnPropertyChanged("Password");
            OnPropertyChanged("Url");
        }
    }
}
