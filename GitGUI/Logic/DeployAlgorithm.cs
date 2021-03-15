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
        List<Tuple<Commit, int>> Result { get; } = new List<Tuple<Commit, int>>();
        Dictionary<int, Node> LastOnRow { get; } = new Dictionary<int, Node>();

        void Add(Node n)
        {
            Result.Add(new Tuple<Commit, int>(n.Commit, n.Row));
            LastOnRow[n.Row] = n;
        }

        public List<Tuple<Commit, int>> ComputeRows(IEnumerable<Commit> c, BranchCollection b)
        {
            if (!c.Any())
                return Result;
            List<Node> nodes = Nodes(c);
            Add(nodes.First());
            HashSet<Commit> branches = new HashSet<Commit>(b.Select(branch => branch.Tip));
            IEnumerable<Node> branchTips = nodes.Where(node => branches.Contains(node.Commit)).ToList();
            foreach (Node n in nodes.Skip(1))
            {
                List<Node> possibleDescOnSameRow = n.Descendants.Where(d =>
                    !d.HasPredecessorOnSameRow && (
                        !branchTips.Contains(d) ||
                        d.Predecessors.Count == 1 ||
                        d.Predecessors.Count - 1 == d.DeployedPredecessors)).ToList();
                List<int> complement = n.Descendants.Except(possibleDescOnSameRow).Select(x => x.Row).ToList();
                possibleDescOnSameRow.RemoveAll(d => complement.Contains(d.Row));
                if (possibleDescOnSameRow.Any())
                {
                    Node descOnSameRow = possibleDescOnSameRow.Aggregate((n1, n2) => (n1.Row < n2.Row) ? n1 : n2);
                    n.Row = descOnSameRow.Row;
                    descOnSameRow.HasPredecessorOnSameRow = true;
                }
                else
                {
                    int i;
                    for (i = 0; i < LastOnRow.Count; i++)
                        if ((LastOnRow[i].DeployedPredecessors == LastOnRow[i].Predecessors.Count) && LastOnRow[i].HasPredecessorOnSameRow == false)
                            break;
                    n.Row = i;
                }
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
