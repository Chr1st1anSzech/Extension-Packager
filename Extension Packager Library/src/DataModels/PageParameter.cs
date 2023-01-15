using System;
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
        public bool _isUpdate { get; set; } = false;
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
    }
}
