using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    class GraphEdge : GraphX.Common.Models.EdgeBase<CommitNode>
    {
        public GraphEdge(CommitNode source, CommitNode target, double weight = 1) : base(source, target, weight) { }

        public GraphEdge() : base(null, null, 1) {  }
    }
}
