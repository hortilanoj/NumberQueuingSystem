using System;
using System.Windows.Input;

namespace COMMON
{
    public class RelayCommand : ICommand
    {
        #region Attributes

        private readonly Action<object> execute;
        private readonly Predicate<object> canExecute;

        #endregion

        #region Events

        public event EventHandler CanExecuteChanged;

        #endregion

        #region Public Methods

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            if (canExecute == null)
                throw new ArgumentNullException("execute");

            this.execute = execute;
            this.canExecute = canExecute;
        }

        public RelayCommand(Action<object> execute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            this.execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            if (this.canExecute == null)
            {
                return true;
            }
            else
            {
                return this.canExecute(parameter);
            }
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            this.OnCanExecuteChanged();
        }

        protected virtual void OnCanExecuteChanged()
        {
            EventHandler handler = this.CanExecuteChanged;
            if (handler != null)
            {
                handler.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
