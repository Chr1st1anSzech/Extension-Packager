// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager_Library.src.Extension;
using Extension_Packager_Library.src.Helper;
using Extension_Packager_Library.src.Navigation;

namespace Extension_Packager_Library.src.Viewmodels
{
    public class SuccessPageViewModel : ViewModel
    {
        #region Public Properties

        private bool _isPageBack;
        public bool IsPageBack
        {
            get { return _isPageBack; }
            set { SetField(ref _isPageBack, value); }
        }


        private bool _isUpdate;
        public bool IsUpdate
        {
            get { return _isUpdate; }
            set { SetField(ref _isUpdate, value); }
        }

        private INavigationService _navigationService;
        public INavigationService NavigationService
        {
            get { return _navigationService; }
            set { SetField(ref _navigationService, value); }
        }


        private DataModels.Extension _extension;
        public DataModels.Extension Extension
        {
            get { return _extension; }
            set { SetField(ref _extension, value); }
        }


        private string _policyString = string.Empty;
        public string PolicyString
        {
            get { return _policyString; }
            set { SetField(ref _policyString, value); }

        }

        #endregion


        #region Private Fields

        #endregion


        #region Commands


        public MyCommand NewExtensionCommand { get; set; }
        public MyCommand BackToStartpageCommand { get; set; }


        private void SetCommands()
        {
            NewExtensionCommand = new MyCommand(NewExtension);
            BackToStartpageCommand = new MyCommand(BackToStartpage);
        }


        #endregion


        public void Init()
        {
            SetCommands();
            SetProperties();
        }


        private void SetProperties()
        {
            PolicyString= Extension.PolicyString ?? string.Empty;
        }

        private void NewExtension(object parameter = null)
        {
            _navigationService.Navigate("CrxSelectPage");
        }

        private void BackToStartpage(object parameter = null)
        {
            _navigationService.Navigate("MainPage");
        }
    }
}
