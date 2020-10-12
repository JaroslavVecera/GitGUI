using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GitGUI
{
    /// <summary>
    /// Interakční logika pro CommitNode.xaml
    /// </summary>
    public partial class CommitNodeView : UserControl
    {
        public PathFigureCollection OutlineSegments { get { return ShapePath(); } }
        public Thickness GridMargin { get { return GridMarginThickness(); } }
        new double MaxWidth { get { return 2 * _margin + _textLength + Height * 3 / 2; } }
        protected double _margin = 8;
        protected double _textLength;
        public bool EnabledPhoto { get { return true; } }

        public CommitNodeView()
        {
            InitializeComponent();
        }

        public Thickness EllipseMargin { get { return EllipseMarginThickness(); } }

        Thickness EllipseMarginThickness()
        {
            return new Thickness(Height * 0.95 - _margin, 0, 0, 0);
        }

        Thickness GridMarginThickness()
        {
            return new Thickness(-_textLength / 2, Height / 2, 0, 0);
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
                Point = new Point(textLength + _margin, Height),
                Size = new Size(Height / 2, Height / 2),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = true
            };
        }

        LineSegment Bottom(bool hasPhoto)
        {
            return new LineSegment(new Point(-_margin - (hasPhoto ? Height / 2 : 0), Height), false);
        }

        ArcSegment LeftArc(bool hasPhoto)
        {
            return new ArcSegment()
            {
                Point = new Point(-_margin - (hasPhoto ? Height / 2 : 0), 0),
                Size = new Size(Height / 2, Height / 2),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = true
            };
        }
    }
}
