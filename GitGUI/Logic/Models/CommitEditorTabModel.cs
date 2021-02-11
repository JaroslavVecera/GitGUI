using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    public class CommitEditorTabModel : TabModel
    {
        public override string Header { get { return "Create commit"; } }
        public event Action<string, IEnumerable<string>> CommitRequest;
        public event Action RepositoryStatusChanged;
        public bool IsChecked { get; set; } = true;
        public string Message { get; set; } = "";
        public RepositoryStatus RepositoryStatus { get; private set; }
        public TreeChanges RepositoryChanges { get; private set; }
        IEnumerable<string> _paths;
        public IEnumerable<string> Paths { get { return _paths; } set { _paths = Adjust(value); } }

        public IEnumerable<string> Adjust(IEnumerable<string> paths)
        {
            if (!paths.Any())
                return paths;
            if (paths.First() == "All")
                return new List<string> { "*" };
            return paths.ToList().Select(p => p.Substring(4));
        }

        public CommitEditorTabModel()
        {
            Refresh();
            LibGitService.GetInstance().RepositoryChanged += Refresh;
        }

        void Refresh()
        {
            RepositoryStatus = LibGitService.GetInstance().Status;
            RepositoryChanges = LibGitService.GetInstance().CurrentChanges;
            RepositoryStatusChanged?.Invoke();
        }

        public void Commit()
        {
            CommitRequest?.Invoke(Message, Paths);
        }
    }
}
