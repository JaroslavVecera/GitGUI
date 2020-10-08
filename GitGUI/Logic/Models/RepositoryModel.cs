using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace GitGUI.Logic
{
    class RepositoryModel : ModelBase
    {
        public string Path { get; private set; }
        public string Name { get { return System.IO.Path.GetDirectoryName(Path); } }
        public DateTimeOffset LastUse { get; set; }

        public RepositoryModel(string path)
        {
            Path = path;
        }
    }
}
