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
        public string RepositoryPath { get; set; }
        public string Name { get { return System.IO.Path.GetFileName(RepositoryPath); } }
        DateTimeOffset _lastUse;
        public DateTimeOffset LastUse { get { return _lastUse; } set { _lastUse = value; Save(); } }
        string StringLastUse { get { return LastUse.ToString(); } set { _lastUse = DateTimeOffset.Parse(value); } }
        public string Path { get; set; }

        public RepositoryModel(string path)
        {
            Path = path;
        }

        public void Load()
        {
            using (StreamReader r = new StreamReader(Path))
            {
                RepositoryPath = r.ReadLine();
                StringLastUse = r.ReadLine();
            }
        }

        void Save()
        {
            using (StreamWriter w = new StreamWriter(Path))
            {
                w.WriteLine(RepositoryPath);
                w.WriteLine(StringLastUse);
            }
        }
    }
}
