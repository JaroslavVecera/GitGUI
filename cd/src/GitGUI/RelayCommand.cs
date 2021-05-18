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

    public class RelayCommand<T> : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action<T> _toExecute;
        private Func<T, bool> _canExecute;
        public RelayCommand(Action<T> methodToExecute, Func<T, bool> canExecuteEvaluator)
        {
            _toExecute = methodToExecute;
            _canExecute = canExecuteEvaluator;
        }

        public RelayCommand(Action<T> methodToExecute) : this(methodToExecute, null) { }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public bool CanExecute(object parameter)
        {
            if (parameter != null && !(parameter is T))
                throw new ArgumentException("Bad type of parameter in this command.");
            if (_canExecute != null)
                return _canExecute((T)parameter);
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter != null && !(parameter is T))
                throw new ArgumentException("Bad type of parameter in this command.");
            _toExecute.Invoke((T)parameter);
        }
    }
}
