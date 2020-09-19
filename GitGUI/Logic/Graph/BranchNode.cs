using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LibGit2Sharp;

namespace GitGUI.Logic
{
    public class BranchNode : Node
    {
        public static double ArrowHeight { get { return 8; } }
        double _arrowHalfWidth = 8;
        public Branch Branch { get; private set; }
        public CommitNode LinkedCommitNode { get; set; }
        public override CommitNode RepresentedNode { get { return LinkedCommitNode; } }
        public Thickness GridMargin { get { return GridMarginThickness(); } }
        public Thickness PolygonMargin { get { return PolygonMarginThickness(); } }
        public PathGeometry Geometry{ get { return Shape(); } }
        public override bool Marked { set { if (value) StyleAsMarked(); else StyleAsUnmarked(); } }
        
        void StyleAsMarked()
        {
            ((GitGUI.BranchNode)GElement).Path.Stroke = Brushes.Black;

        }

        void StyleAsUnmarked()
        {
            ((GitGUI.BranchNode)GElement).Path.Stroke = Brushes.Transparent;
        }

        public BranchNode(Branch b)
        {
            Branch = b;
            _maxWidth = 100;
            SetGElement();
        }

        void SetGElement()
        {
            GitGUI.BranchNode n = new GitGUI.BranchNode();
            ((MainWindow)Application.Current.MainWindow).zoomCanvas.Children.Add(n);
            n.DataContext = this;
            GElement = n;
            OnNameChanged();
            ((GitGUI.BranchNode)GElement).Path.StrokeThickness = 3;
        }

        void OnNameChanged()
        {
            ((GitGUI.BranchNode)GElement).TextBlock.Text = Branch.Name;
            RequestRebuild();
        }

        void RequestRebuild()
        {
            TextBlock b = ((GitGUI.BranchNode)GElement).TextBlock;
            MeasureTextWidth(b);
            b.Width = _textLength;
            OnChanged("GridMargin", "OutlineSegments", "PolygonMargin", "Rectangle");
        }

        PathGeometry Shape()
        {
            return new PathGeometry(new List<PathFigure>() { new PathFigure(new Point(-_margin, 0), Segments(), true) });
        }

        List<PathSegment> Segments()
        {
            List<PathSegment> l = new List<PathSegment>() { Left(), Bottom(), Right(), Top() };
            l.AddRange(Arrow());
            return l;
        }

        List<PathSegment> Arrow()
        {
            List<PathSegment> res = new List<PathSegment>();
            if (RepresentedNode.Branches[0] == this)
            {
                res.Add(new LineSegment() { Point = new Point(_textLength / 2, -ArrowHeight) });
                res.Add(new LineSegment() { Point = new Point(_textLength / 2 - _arrowHalfWidth, 0) });
            }
            return res;
        }

        LineSegment Top()
        {
            return new LineSegment() { Point = new Point(_textLength / 2 + _arrowHalfWidth, 0) };
        }

        LineSegment Right()
        {
            return new LineSegment { Point = new Point(_margin + _textLength, 0) };
        }

        LineSegment Bottom()
        {
            return new LineSegment { Point = new Point(_margin + _textLength, GElement.Height) };
        }

        LineSegment Left()
        {
            return new LineSegment() { Point = new Point(-_margin, GElement.Height) };
        }

        Thickness GridMarginThickness()
        {
            return new Thickness(-_textLength / 2, -GElement.Height / 2, 0, 0);
        }

        Thickness PolygonMarginThickness()
        {
            return new Thickness(_textLength / 2, 0, 0, 0);
        }
    }
}
