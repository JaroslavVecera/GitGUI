using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    public abstract class ChangesTreeItem : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public event Action Checked;
        public event Action Unchecked;
        public event PropertyChangedEventHandler PropertyChanged;
        
        bool _isChecked = true;

        public virtual bool IsChecked { get { return _isChecked; }
            set { _isChecked = value; Notify(value); } }

        protected ChangesInfo _info;

        public virtual ChangesInfo Info { get { return null; } set { _info = value; } }

        public abstract IEnumerable<string> GetCheckedPaths(string prefix);

        public abstract IEnumerable<string> GetUncheckedPaths(string prefix);

        void Notify(bool val)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsChecked"));
            if (val)
                Checked?.Invoke();
            else
                Unchecked?.Invoke();
        }
    }
}
