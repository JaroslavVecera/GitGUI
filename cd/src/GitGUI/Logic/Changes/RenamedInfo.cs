using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    public class RenamedInfo :ChangesInfo
    {
        public string OldName { get; private set; }
        public string NewName { get; private set; }

        public RenamedInfo(string oldPath, string newPath)
        {
            OldName = oldPath;
            NewName = newPath;
        }
    }
}
