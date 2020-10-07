using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace GitGUI.Logic
{
    class StashingManager
    {
        StashCollection _stashes;
        string _dirPath;

        public void SetRepository(Repository r)
        {
            _stashes = r.Stashes;
            _dirPath = r.Info.Path;
        }

        public void ImplicitPush(CommitNodeModel model)
        {

        }

        public void ImplicitPop(CommitNodeModel model)
        {

        }

        public void Pop(CommitNodeModel model)
        {

        }

        public void PopLast()
        {
            _stashing.Pop(0);
        }

        public void Push(CommitNodeModel model)
        {

        }

        int FindIndex(CommitNodeModel model)
        {
            throw new NotImplementedException();
        }
    }
}
