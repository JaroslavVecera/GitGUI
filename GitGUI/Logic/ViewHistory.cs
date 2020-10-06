using System;
using System.Collections.Generic;
using System.Linq;

namespace GitGUI.Logic
{
    class ViewHistory
    {
        LinkedList<GraphItemModel> History { get; } = new LinkedList<GraphItemModel>();
        LinkedList<GraphItemModel> Future { get; } = new LinkedList<GraphItemModel>();

        public void Add(GraphItemModel node)
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

        public GraphItemModel Next()
        {
            if (!Future.Any())
                throw new InvalidOperationException("ViewHistory has not any future.");
            GraphItemModel n = History.First();
            History.RemoveFirst();
            Future.AddFirst(n);
            return n;
        }

        public GraphItemModel Previous()
        {
            if (!History.Any())
                throw new InvalidOperationException("ViewHistory has not any history.");
            GraphItemModel n = Future.First();
            Future.RemoveFirst();
            History.AddFirst(n);
            return n;
        }

        public void EnsureAbsence(GraphItemModel n)
        {
            History.Remove(n);
            Future.Remove(n);
        }
    }
}
