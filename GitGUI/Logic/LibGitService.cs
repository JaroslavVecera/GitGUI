using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace GitGUI.Logic
{
    class LibGitService
    {
        Repository Repository { get; set; }
        public BranchLabelModel CurrentBranch { get; }

        public Repository OpenNewRepository(string path)
        {
            try
            {
                Repository.Init(path);
            }
            catch (LibGit2SharpException e)
            {
                throw new NotImplementedException();
            }
            return OpenRepository(path);
        }

        public void Add(List<string> files)
        {
            Commands.Stage(Repository, files);
        }

        public Repository OpenRepository(string path)
        {
            if (Repository != null)
                CloseRepository(Repository);
            Repository = new Repository(path);
            return Repository;
        }

        public void Merge(BranchLabelModel merging, BranchLabelModel merged)
        {
            Signature s = new Signature(new Identity("anonym", "anonym@upol.cz"), DateTimeOffset.Now);
            MergeResult r = Repository.Merge(merging.Branch, s);
        }

        public void Commit(BranchLabelModel l, string message)
        {
            Signature s = new Signature(new Identity("anonym", "anonym@upol.cz"), DateTimeOffset.Now);
            Commit c = Repository.Commit(message, s, s);
        }

        public void Branch(GraphItemModel i, string name)
        {
            if (i != null)
                CreateBranch(name, (dynamic)i);
            else
                Repository.CreateBranch(name);
        }

        public void Checkout(BranchLabelModel b)
        {
            Commands.Checkout(Repository, b.Branch);
        }

        void CloseRepository(Repository r)
        {
            Repository.Dispose();
            Repository = null;
        }

        Branch CreateBranch(string name, BranchLabelModel l)
        {
            return Repository.CreateBranch(name, l.Branch.Tip);
        }

        Branch CreateBranch(string name, CommitNodeModel n)
        {
            return Repository.CreateBranch(name, n.Commit);
        }
    }
}
