using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GitGUI
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action _toExecute;
        private Func<bool> _canExecute;
        public RelayCommand(Action methodToExecute, Func<bool> canExecuteEvaluator)
        {
            _toExecute = methodToExecute;
            _canExecute = canExecuteEvaluator;
        }

        public RelayCommand(Action methodToExecute) : this(methodToExecute, null) { }

        public void RaiseCanExecuteChanged()
        {
                CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute();
            return true;
        }

        public void Execute(object parameter)
        {
            _toExecute.Invoke();
        }
    }
}
