using System;
using System.Collections.Generic;
using System.Linq;

namespace GitGUI.Logic
{
    class ViewHistory
    {
        LinkedList<Node> History { get; } = new LinkedList<Node>();
        LinkedList<Node> Future { get; } = new LinkedList<Node>();

        public void Add(Node node)
        {
            Future.Clear();
            History.AddFirst(node);
        }

        public bool HasHistory()
        {
            return History.Any();
        }

        public bool HasFuture()
        {
            return Future.Any();
        }

        public Node Next()
        {
            if (!Future.Any())
                throw new InvalidOperationException("ViewHistory has not any future.");
            Node n = History.First();
            History.RemoveFirst();
            Future.AddFirst(n);
            return n;
        }

        public Node Previous()
        {
            if (!History.Any())
                throw new InvalidOperationException("ViewHistory has not any history.");
            Node n = Future.First();
            Future.RemoveFirst();
            History.AddFirst(n);
            return n;
        }

        public void EnsureAbsence(Node n)
        {
            History.Remove(n);
            Future.Remove(n);
        }
    }
}
