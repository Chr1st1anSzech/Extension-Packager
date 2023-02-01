// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Extension_Packager_Library.src.Helper
{
    public class ViewModel : INotifyPropertyChanged
    {
        #region Public Properties


        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetField(ref _isBusy, value); }
        }

        private string _warningMessage;
        public string WarningMessage
        {
            get { return _warningMessage; }
            set { SetField(ref _warningMessage, value); }
        }

        private bool _isWarningVisible;
        public bool IsWarningVisible
        {
            get { return _isWarningVisible; }
            set { SetField(ref _isWarningVisible, value); }
        }


        #endregion


        #region Static Fields

        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }


        public bool IsValidOrWarn(Func<bool> ValidateFunc, string errorMessage)
        {
            bool isValid = ValidateFunc();
            if (!isValid)
            {
                ShowWarning(errorMessage);
            }
            return isValid;
        }


        public void ShowWarning(string message, Exception exception = null)
        {
            IsBusy = false;
            WarningMessage = message;
            IsWarningVisible = true;
            if (exception != null)
            {
                _log.Warn(message, exception);
            }
            else
            {
                _log.Warn(message);
            }
        }
    }
}
