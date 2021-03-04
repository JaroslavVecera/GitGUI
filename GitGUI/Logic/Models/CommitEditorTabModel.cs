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
        public event Action<string, IEnumerable<string>, IEnumerable<string>> CommitRequest;
        public event Action RepositoryStatusChanged;
        public bool IsChecked { get; set; } = true;
        public string Message { get; set; } = "";
        public RepositoryStatus RepositoryStatus { get; private set; }
        public TreeChanges RepositoryChanges { get; private set; }
        IEnumerable<string> _staged;
        IEnumerable<string> _unstaged;
        public IEnumerable<string> Staged { get { return _staged; } set { _staged = Adjust(value); } }
        public IEnumerable<string> Unstaged { get { return _unstaged; } set { _unstaged = Adjust(value); } }

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

        public void FreeEvents()
        {
            LibGitService.GetInstance().RepositoryChanged -= Refresh;
        }

        void Refresh()
        {
            RepositoryStatus = LibGitService.GetInstance().Status;
            RepositoryChanges = LibGitService.GetInstance().CurrentChanges;
            RepositoryStatusChanged?.Invoke();
        }

        public void Commit()
        {
            CommitRequest?.Invoke(Message, Staged, Unstaged);
        }
    }
}
