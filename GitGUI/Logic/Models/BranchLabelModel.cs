using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using LibGit2Sharp;

namespace GitGUI
{
    public class BranchLabelModel : GraphItemModel
    {
        MatrixTransform _transform;
        Point _loc;
        bool _hitTestVisible = true;
        public Point Location { get { return _loc; } set { _loc = value; OnPropertyChanged(); } }
        public bool Checkouted { get; set; }
        public Branch Branch { get; set; }
        public MatrixTransform RenderTransform
        {
            get { return _transform; }
            set { _transform = value;  OnPropertyChanged(); }
        }
        public bool IsHitTestVisible
        {
            get { return _hitTestVisible; }
            set { _hitTestVisible = value; OnPropertyChanged(); }
        }

        public void BackgroundPush()
        {

        }

        public void ForegroundPull()
        {

        }
    }
}
