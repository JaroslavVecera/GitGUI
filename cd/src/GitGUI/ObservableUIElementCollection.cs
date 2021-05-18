using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows;

namespace GitGUI
{
    public class ObservableUIElementCollection : UIElementCollection
    {
        public ObservableUIElementCollection(UIElement visualParent, FrameworkElement logicalParent)
            : base(visualParent, logicalParent) { }

        public delegate void UIElementAddHandler(UIElement sender);

        public event UIElementAddHandler AddedUIElement;

        public override int Add(UIElement element)
        {
            int pos = base.Add(element);
            OnUIElementAdd(element);
            return pos;
        }

        protected virtual void OnUIElementAdd(UIElement e)
        {
            UIElementAddHandler handler = AddedUIElement;
            handler?.Invoke(e);
        }
    }
}
