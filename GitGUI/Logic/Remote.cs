using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    class Remote
    {
        public string Name { get; private set; }
        public string Url { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }

        public Remote()
        {
            InitializeCommands();
        }

        public Remote(string name, string url) : this(name, url, "", "") { }

        public Remote(string name, string url, string userName, string password)
        {
            Name = name;
            Url = url;
            UserName = userName;
            Password = password;
            InitializeCommands();
        }

        void InitializeCommands()
        {

        }
    }
}
