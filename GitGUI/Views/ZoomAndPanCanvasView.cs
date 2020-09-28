using System;
using System.Collections.Generic;
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
    public class ZoomAndPanCanvasView : Canvas
    {
        static ZoomAndPanCanvasView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomAndPanCanvasView), new FrameworkPropertyMetadata(typeof(ZoomAndPanCanvasView)));
        }

        protected override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent)
        {
            ObservableUIElementCollection c = new ObservableUIElementCollection(this, logicalParent);
            c.AddedUIElement += OnChildAdd;
            return c;
        }

        void OnChildAdd(UIElement sender)
        {
        }
    }
}
