// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager.src.Helpers;
using Extension_Packager_Library.src.Extension;
using Extension_Packager_Library.src.Navigation;

namespace Extension_Packager_Library.src.Viewmodels
{
    public class SuccessPageViewModel : ViewModel
    {
        #region Public Properties

        private INavigationService _navigationService;
        public INavigationService NavigationService
        {
            get { return _navigationService; }
            set { SetField(ref _navigationService, value); }
        }

        private string _policyString = string.Empty;
        public string PolicyString
        {
            get { return _policyString; }
            set { SetField(ref _policyString, value); }

        }

        #endregion


        #region Private Fields

        private readonly DataModels.Extension _ext = ExtensionManager.Instance.CurrentExtension;

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


        public SuccessPageViewModel()
        {
            SetCommands();
            SetProperties();
        }


        private void SetProperties()
        {
            PolicyString= _ext.PolicyString;
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
