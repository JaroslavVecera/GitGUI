using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    public class CommitViewerTabModel : TabModel
    {
        public override string Header { get { return Commit.Message.TrimEnd(new char[] { '\n', '\r' }); } }
        public event Action RepositoryStatusChanged;
        public CommitNodeModel Commit { get; private set; }
        public string Message { get { return Commit.Message; } }
        public TreeChanges CommitChanges { get; private set; }
        public int SelectedCommitIndex { set { SelectDiff(value); } }
        public Commit SelectedCommit { get; set; }

        public CommitViewerTabModel(CommitNodeModel m)
        {
            Commit = m;
            Refresh();
            LibGitService.GetInstance().RepositoryChanged += Refresh;
        }

        void SelectDiff(int index)
        {
            if (index < 0 || !Commit.Commit.Parents.Any())
                return;
            SelectedCommit = Commit.Commit.Parents.ToList()[index];
            CommitChanges = LibGitService.GetInstance().CommitChanges(Commit.Commit, SelectedCommit);
            RepositoryStatusChanged?.Invoke();
        }

        public void FreeEvents()
        {
            LibGitService.GetInstance().RepositoryChanged -= Refresh;
        }

        void Refresh()
        {
            CommitChanges = LibGitService.GetInstance().CommitChanges(Commit.Commit);
            RepositoryStatusChanged?.Invoke();
        }
    }
}
