using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    class StashingManager : LibGit2Sharp.StashCollection
    {
        public void SaveForCommit(CommitNode n)
        {

        }

        public void LoadForCommit(CommitNode n)
        {

        }

        public bool HasStash(CommitNode n)
        {
            throw new NotImplementedException();
        }
    }
}
