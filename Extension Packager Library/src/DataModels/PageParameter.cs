using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Extension_Packager_Library.src.DataModels
{
    public class PageParameter : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Extension Extension { get; set; }

        private bool _isUpdate = false;
        public bool IsUpdate
        {
            get
            {
                return _isUpdate;
            }

            set
            {
                if (value != _isUpdate)
                {
                    _isUpdate = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _cancelNavigation = false;
        public bool CancelNavigation
        {
            get
            {
                return _cancelNavigation;
            }

            set
            {
                if (value != _cancelNavigation)
                {
                    _cancelNavigation = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isPageBack = false;
        public bool IsPageBack
        {
            get
            {
                return _isPageBack;
            }

            set
            {
                if (value != _isPageBack)
                {
                    _isPageBack = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isAddition = false;
        public bool IsAddition
        {
            get
            {
                return _isAddition;
            }

            set
            {
                if (value != _isAddition)
                {
                    _isAddition = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Temporary values that are need during the packaging process.
        /// </summary>
        private readonly Dictionary<string, object> _tmpValues = new();

        internal T Get<T>(string key)
        {

            return _tmpValues.ContainsKey(key) && _tmpValues[key] is T t ? t : default;
        }

        internal T TakeOut<T>(string key)
        {
            T val = _tmpValues.ContainsKey(key) && _tmpValues[key] is T t ? t : default;
            _tmpValues.Remove(key);
            return val;
        }

        internal void Set(string key, object value)
        {
            if (_tmpValues.ContainsKey(key))
            {
                _tmpValues[key] = value;
            }
            else
            {
                _tmpValues.Add(key, value);
            }
        }
    }
}
