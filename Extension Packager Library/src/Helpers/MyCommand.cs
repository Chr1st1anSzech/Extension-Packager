// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Windows.Input;

namespace Extension_Packager.src.Helpers
{
    public class MyCommand : ICommand
    {
        private readonly Action<object> _targetExecuteMethod;
        private readonly Func<object, bool> _targetCanExecuteMethod;

        public MyCommand(Action<object> executeMethod)
        {
            _targetExecuteMethod = executeMethod;
        }

        public MyCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
        {
            _targetExecuteMethod = executeMethod;
            _targetCanExecuteMethod = canExecuteMethod;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        bool ICommand.CanExecute(object parameter)
        {
            if (_targetCanExecuteMethod != null)
            {
                return _targetCanExecuteMethod(parameter);
            }

            if (_targetExecuteMethod != null)
            {
                return true;
            }

            return false;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        void ICommand.Execute(object parameter)
        {
            _targetExecuteMethod?.Invoke(parameter);
        }
    }
}
