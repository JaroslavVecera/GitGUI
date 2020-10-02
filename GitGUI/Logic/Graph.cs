using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GitGUI.Logic
{
    class Graph
    {
        public GraphItemModel Marked { get; set; }
        public BranchLabelModel Checkouted { get; set; }
        static Graph Instance { get; set; } = new Graph();
        double ScaleFactor { get; set; } = 1;
        Point Center { get { return GraphViewCenter(); } }
        ZoomAndPanCanvasModel ZoomAndPanCanvasModel { get; set; }
        public LibGit2Sharp.Repository Repository { private get; set; }
        public EventHandlerBatch EventHandlerBatch { private get; set; }

        public void Move(Vector move)
        {
            ZoomAndPanCanvasModel.Move(move);
        }

        public void Scale(int wheelDelta, Point mouse)
        {
            double scaleFactor = wheelDelta > 0 ? 1.25 : 0.8;
            AppSettings set = ((App)Application.Current).Settings;
            Point origin = set.UseMouseAsZoomOrigin ? mouse : Center;
            ZoomAndPanCanvasModel.Rescale(scaleFactor, origin);
        }
        
        public void HighlightAsMarked(GraphItemModel model)
        {
            if (Marked != null)
                Marked.Marked = false;
            Marked = model;
            if (model != null)
                model.Marked = true;
        }

        public void HighlightAsCheckouted(BranchLabelModel branch)
        {
            if (Checkouted != null)
                Checkouted.Checkouted = false;
            Checkouted = branch;
            branch.Checkouted = true;
        }

        Point GraphViewCenter()
        {
            ScrollViewer g = ((MainWindow)Application.Current.MainWindow).graphView;
            return new Point((double)g.ActualWidth / 2, (double)g.ActualHeight / 2);
        }

        public void DeployGraph()
        {
            DeployCommitNodes();
            DeployBranchNodes();
        }

        void DeployBranchNodes()
        {

        }

        void DeployCommitNodes()
        {

        }

        void SubscribeEvents(GraphItemModel m)
        {
            m.MouseDown += EventHandlerBatch.MouseDownEventHandler;
            m.MouseEnter += EventHandlerBatch.MouseEnterEventHandler;
            m.MouseLeave += EventHandlerBatch.MouseLeaveEventHandler;
        }

        public static Graph GetInstance()
        {
            return Instance;
        }
    }
}
