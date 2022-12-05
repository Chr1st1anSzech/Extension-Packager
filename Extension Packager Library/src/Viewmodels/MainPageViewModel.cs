// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager.src.Helpers;
using Extension_Packager_Library.src.Navigation;
using System.Collections.Generic;

namespace Extension_Packager_Library.src.Viewmodels
{
    public class MainPageViewModel : ViewModel
    {
        #region Public Properties
        
        public List<string> BrowserFilterList { get; } = new() { "Alle", "Google Chrome", "Microsoft Edge" };
       
        private INavigationService _navigationService;
        public INavigationService NavigationService
        {
            get { return _navigationService; }
            set { SetField(ref _navigationService, value); }
        }

        #endregion


        #region Commands

        public MyCommand SettingsCommand { get; set; }
        public MyCommand CreateExtensionCommand { get; set; }
        public MyCommand ModifyExtensionCommand { get; set; }
        
        private void SetCommands()
        {
            SettingsCommand = new MyCommand(OpenSettings);
            CreateExtensionCommand = new MyCommand(CreateExtension);
            ModifyExtensionCommand = new MyCommand(ModifyExtension);
        }

        #endregion


        public MainPageViewModel()
        {
            SetCommands();
        }


        private void OpenSettings(object parameter = null)
        {

        }

        private void CreateExtension(object parameter = null)
        {
            _navigationService.Navigate("CrxSelectPage");
        }

        private void ModifyExtension(object parameter = null)
        {

        }
    }
}
