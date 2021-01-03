using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using LibGit2Sharp;

namespace GitGUI.Logic
{
    public class BranchLabelModel : GraphItemModel
    {
        MatrixTransform _transform;
        bool _hitTestVisible = true;
        public Branch Branch { get; set; }
        public string Name { get { return Branch.FriendlyName; } }

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
    }
}
