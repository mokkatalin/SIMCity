﻿using System;
using System.Windows.Input;

namespace CityBuilder.ViewModel
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<Object> _execute; // a tevékenységet végrehajtó lambda-kifejezés
        private readonly Func<Object, Boolean> _canExecute;
        
        public DelegateCommand(Action<Object> execute) : this(null, execute) { }
        public DelegateCommand(Func<Object, Boolean> canExecute, Action<object> execute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            _execute = execute;
            _canExecute = canExecute;
        }
        
        public event EventHandler? CanExecuteChanged;
        public Boolean CanExecute(Object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public void Execute(Object parameter)
        {
            if (!CanExecute(parameter))
            {
                throw new InvalidOperationException("Command execution is disabled.");
            }
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}
