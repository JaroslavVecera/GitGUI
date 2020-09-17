using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    class StateRelatedData
    {
        Point _point;
        public Node AttachedNode { get; set; }
        public Point MousePoint { get { return _point; } set { PreviousMousePoint = _point; _point = value; } }
        Point PreviousMousePoint { get; set; }
        public Vector MouseDisplacement { get { return Point.Subtract(MousePoint, PreviousMousePoint); } }
    }
}
