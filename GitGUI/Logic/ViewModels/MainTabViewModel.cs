using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GitGUI.Logic
{
    public class MainTabViewModel : TabViewModel
    {
        MainTabModel Model { get; set; }

        public string Header { get { return "Main"; } }
        public ZoomAndPanCanvasView ZoomAndPanCanvas { get; private set; }
        public RelayCommand MouseDown { get; private set; }
        public MouseButtonEventArgs MouseButtonArgs { get; set; }
        double _height = 250, _width = 250;
        public ScrollViewer ScrollViewer { get; set; }
        public Point GraphViewCenter { get { return new Point(ScrollViewer.ActualWidth / 2, ScrollViewer.ActualHeight / 2); } }
        bool _isInfoVisible;
        public bool IsInfoVisible { get { return Model.Shown != null; } }
        public GraphItemModel Shown { get { return Model.Shown; } }

        void SetGraphViewCenter()
        {
            Model.GraphViewCenter = new Point(_width, _height);
        }

        public ActionPanelViewModel ActionPanel
        { get { return new ActionPanelViewModel(Model.PanelModel); } }

        public MainTabViewModel(MainTabModel model) : base(model)
        {
            ZoomAndPanCanvas = new ZoomAndPanCanvasView();
            new ZoomAndPanCanvasViewModel(Graph.GetInstance().ZoomAndPanCanvasModel, ZoomAndPanCanvas);
            MouseDown = new RelayCommand(() => Model.OnMouseDown(MouseButtonArgs));
            Model = model;
            SubscribeModel(model);
        }

        void SubscribeModel(MainTabModel model)
        {
            Model.UpdateCenter += () => Model.GraphViewCenter = GraphViewCenter;
            Model.ShownChanged += () => { OnPropertyChanged("IsInfoVisible"); OnPropertyChanged("Shown"); };
            Model.PropertyChanged += (sender, e) => OnPropertyChanged(e.PropertyName);
        }
    }
}
