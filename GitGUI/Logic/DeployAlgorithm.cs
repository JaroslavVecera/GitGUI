using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace GitGUI.Logic
{
    public class DeployAlgorithm
    {
        private int _nextRow = 0;
        List<Tuple<Commit, int>> Result { get; } = new List<Tuple<Commit, int>>();

        void Add(Node n)
        {
            Result.Add(new Tuple<Commit, int>(n.Commit, n.Row));
        }

        public List<Tuple<Commit, int>> ComputeRows(IEnumerable<Commit> c, BranchCollection b)
        {
            if (!c.Any())
                return Result;
            List<Node> nodes = Nodes(c);
            Add(nodes.First());
            IEnumerable<Node> branchTips = nodes.Where(node => b.Select(branch => branch.Tip).ToList().Contains(node.Commit));
            foreach (Node n in nodes.Skip(1))
            {
                IEnumerable<Node> possibleDescOnSameRow = n.Descendants.Where(d =>
                    !d.HasPredecessorOnSameRow && (
                        !branchTips.Contains(d) ||
                        d.Predecessors.Count == 1 ||
                        d.Predecessors.Count - 1 == d.DeployedPredecessors));
                if (possibleDescOnSameRow.Any())
                {
                    Node descOnSameRow = possibleDescOnSameRow.Aggregate((n1, n2) => (n1.Row < n2.Row) ? n1 : n2);
                    n.Row = descOnSameRow.Row;
                    descOnSameRow.HasPredecessorOnSameRow = true;
                }
                else
                    n.Row = ++_nextRow;
                n.Descendants.ForEach(d => d.DeployedPredecessors++);
                Add(n);
            }
            return Result;
        }

        List<Node> Nodes(IEnumerable<Commit> cl)
        {
            Dictionary<Commit, Node> pairs = new Dictionary<Commit, Node>();
            List<Node> res = new List<Node>();
            foreach (Commit c in cl)
            {
                Node n = new Node(c);
                pairs.Add(c, n);
                res.Add(n);
            }
            foreach (Commit c in cl)
            {
                foreach (Commit p in c.Parents)
                {
                    pairs[c].Predecessors.Add(pairs[p]);
                    pairs[p].Descendants.Add(pairs[c]);
                }
            }
            res.Reverse();
            return res;
        }

        class Node
        {
            public Commit Commit { get; private set; }
            public List<Node> Descendants { get; } = new List<Node>();
            public List<Node> Predecessors { get; } = new List<Node>();
            public int Row { get; set; } = 0;
            public bool HasPredecessorOnSameRow { get; set; } = false;
            public int DeployedPredecessors { get; set; } = 0;

            public Node(Commit c)
            {
                Commit = c;
            }
        }
    }
}
