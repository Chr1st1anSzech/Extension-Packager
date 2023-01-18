// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager_Library.src.DataModels;
using Extension_Packager_Library.src.Extension;
using Extension_Packager_Library.src.Helper;
using Extension_Packager_Library.src.Navigation;

namespace Extension_Packager_Library.src.Viewmodels
{
    public class SuccessPageViewModel : ViewModel
    {
        #region Public Properties

        private PageParameter _pageParameter;
        public PageParameter PageParameter
        {
            get { return _pageParameter; }
            set { SetField(ref _pageParameter, value); }
        }

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
            PolicyString= PageParameter.Get<string>("PolicyString") ?? string.Empty;
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
