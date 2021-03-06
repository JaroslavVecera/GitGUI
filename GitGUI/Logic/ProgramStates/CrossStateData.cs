﻿using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    class CrossStateData
    {
        Point _point;
        public BranchLabelModel AttachedBranch { get; set; }
        public Point MousePoint { get { return _point; } set { PreviousMousePoint = _point; _point = value; } }
        Point PreviousMousePoint { get; set; }
        public Vector MouseDisplacement { get { return Point.Subtract(MousePoint, PreviousMousePoint); } }
        public MainWindowModel MainWindowModel { get; set; }
    }
}
