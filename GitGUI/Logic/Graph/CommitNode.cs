using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;

namespace GitGUI.Logic
{
    class CommitNode : Node
    {
        string _path;
        public Commit Commit { get; private set; }
        public List<BranchNode> Branches { get; } = new List<BranchNode>();
        public override CommitNode RepresentedNode { get { return this; } }
        public Thickness EllipseMargin { get { return EllipseMarginThickness(); } }
        public Thickness GridMargin { get { return GridMarginThickness(); } }
        public PathFigureCollection OutlineSegments { get { return ShapePath(); } }
        public override bool Marked { set { } }
        public GraphX.Measure.Size RotatedVertexSize
        {
            get { return new GraphX.Measure.Size(TotalHeight(), MaxWidth); }
        }
        double MaxWidth { get { return 2 * _margin + _textLength + GElement.Height * 3 / 2; } }

        public bool EnabledPhoto
        {
            get { return Path != null && ((App)Application.Current).Settings.ShowAuthorMiniatures; }
        }

        public string Path
        {
            get { return _path; }
            set { _path = value; OnChanged("Path", "EnabledPhoto"); }
        }

        public CommitNode(Commit c)
        {
            Commit = c;
            _maxWidth = 150;
            SetGElement();
            RequestRebuild();
        }

        void RequestRebuild()
        {
            TextBlock b = ((GitGUI.CommitNode)GElement).TextBlock;
            MeasureTextWidth(b);
            b.Width = _textLength;
            OnChanged("Message", "GridMargin", "OutlineSegments");
        }

        void SetGElement()
        {
            GitGUI.CommitNode c = new GitGUI.CommitNode();
            ((MainWindow)Application.Current.MainWindow).ZoomCanvas.Children.Add(c);
            c.DataContext = this;
            GElement = c;
            OnNameChanged();
            ((GitGUI.CommitNode)GElement).Path.StrokeThickness = 3;
        }

        void OnNameChanged()
        {
            ((GitGUI.CommitNode)GElement).TextBlock.Text = Commit.Message;
            RequestRebuild();
        }

        public void OnChangeProfilPhoto(string path)
        {
            Path = path;
        }

        Thickness EllipseMarginThickness()
        {
            return new Thickness(-GElement.Height * 0.95 - _margin, 0, 0, 0);
        }

        Thickness GridMarginThickness()
        {
            return new Thickness(-_textLength / 2, -GElement.Height / 2, 0, 0);
        }

        PathFigureCollection ShapePath()
        {
            return new PathFigureCollection() { new PathFigure()
            {
                StartPoint = new Point(0, 0),
                Segments = new PathSegmentCollection { Top(_textLength), RightArc(_textLength), Bottom(EnabledPhoto), LeftArc(EnabledPhoto) },
                IsClosed = true
            } };
        }

        LineSegment Top(double textLength)
        {
            return new LineSegment(new Point(textLength + _margin, 0), false);
        }

        ArcSegment RightArc(double textLength)
        {
            return new ArcSegment()
            {
                Point = new Point(textLength + _margin, GElement.Height),
                Size = new Size(GElement.Height / 2, GElement.Height / 2),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = true
            };
        }

        LineSegment Bottom(bool hasPhoto)
        {
            return new LineSegment(new Point(-_margin - (hasPhoto ? GElement.Height / 2 : 0), GElement.Height), false);
        }

        ArcSegment LeftArc(bool hasPhoto)
        {
            return new ArcSegment()
            {
                Point = new Point(-_margin - (hasPhoto ? GElement.Height / 2 : 0), 0),
                Size = new Size(GElement.Height / 2, GElement.Height / 2),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = true
            };
        }

        double TotalHeight()
        {
            double height = GElement.Height;
            Branches.ForEach(b => height += b.GElement.Height);
            height += Branches.Any() ? BranchNode.ArrowHeight : 0;
            return height;
        }
    }
}
