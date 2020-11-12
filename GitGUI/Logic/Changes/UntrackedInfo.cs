using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace GitGUI.Logic
{
    class UntrackedInfo : ChangesInfo
    {
        public string Content { get; private set; }

        public UntrackedInfo(string content)
        {
            Content = content;
        }
    }
}
