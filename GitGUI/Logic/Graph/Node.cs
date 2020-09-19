using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace GitGUI.Logic
{
    public abstract class Node : GraphX.Common.Models.VertexBase, INotifyPropertyChanged
    {
        UserControl _gElement;
        protected double _maxWidth;
        protected double _margin = 8;
        protected double _textLength;
        public bool Shared { get; set; }
        public bool Stashed { get; set; }
        public bool Checkouted { get; set; }
        public abstract CommitNode RepresentedNode { get; }
        public abstract bool Marked { set; }
        public Point Location
        {
            get { return new Point(Canvas.GetLeft(_gElement), Canvas.GetTop(_gElement)); }
            set { Canvas.SetLeft(_gElement, value.X); Canvas.SetTop(_gElement, value.Y); }
        }
        public UserControl GElement
        {
            get { return _gElement; }
            protected set { SetupGElement(value); }
        }

        public delegate void MouseEventHandler(Node sender, MouseEventArgs e);
        public delegate void MouseButtonEventHandler(Node sender, MouseButtonEventArgs e);

        public event MouseButtonEventHandler MouseDown;
        public event MouseEventHandler MouseEnter;
        public event MouseEventHandler MouseLeave;
        public event PropertyChangedEventHandler PropertyChanged;

        public void ForegroundPull()
        {
            Panel.SetZIndex(_gElement, 1);
        }

        protected void OnChanged(params string[] propertyNames)
        {
            propertyNames.ToList().ForEach(name => OnChanged(name));
        }

        protected void OnChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void BackgroundPush()
        {
            Panel.SetZIndex(_gElement, 0);
        }

        protected void MeasureTextWidth(TextBlock b)
        {
            b.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            if (b.DesiredSize.Width <= _maxWidth)
                _textLength = b.DesiredSize.Width;
            else
                _textLength = Math.Min(_maxWidth, b.DesiredSize.Width / 2);
        }

        void SetEvents()
        {
            _gElement.MouseEnter += OnMouseEnter;
            _gElement.MouseLeave += OnMouseLeave;
            _gElement.PreviewMouseDown += OnMouseDown;
        }

        void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            MouseDown?.Invoke(this, e);
        }

        void OnMouseEnter(object sender, MouseEventArgs e)
        {
            MouseEnter?.Invoke(this, e);
        }

        void OnMouseLeave(object sender, MouseEventArgs e)
        {
            MouseLeave?.Invoke(this, e);
        }

        void InitializeLocation()
        {
            Canvas.SetLeft(_gElement, 0);
            Canvas.SetTop(_gElement, 0);
        }

        void SetupGElement(UserControl p)
        {
            _gElement = p;
            SetEvents();
            InitializeLocation();
        }

        public static List<CommitNode> LinkedCommitNodes(List<Node> nodes)
        {
            return nodes.Select(node => node is CommitNode c ? c : ((BranchNode)node).LinkedCommitNode).ToList();
        }
    }
}
