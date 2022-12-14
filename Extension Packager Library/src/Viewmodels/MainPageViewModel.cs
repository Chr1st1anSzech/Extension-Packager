// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager_Library.src.Database;
using Extension_Packager_Library.src.DataModels;
using Extension_Packager_Library.src.Helper;
using Extension_Packager_Library.src.Navigation;
using System;
using System.Collections.Generic;

namespace Extension_Packager_Library.src.Viewmodels
{
    public class MainPageViewModel : ViewModel
    {
        #region Public Properties
        
        public List<string> BrowserFilterList { get; } = new() { "Alle", "Google Chrome", "Microsoft Edge" };

        public List<DataModels.Extension> ExtensionsList { get; set; } = new();

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
        public MyCommand UpdateExtensionCommand { get; set; }

        private void SetCommands()
        {
            SettingsCommand = new MyCommand(OpenSettings);
            CreateExtensionCommand = new MyCommand(CreateExtension);
            ModifyExtensionCommand = new MyCommand(ModifyExtension);
            UpdateExtensionCommand = new MyCommand(UpdateExtension);
        }

        #endregion


        public MainPageViewModel()
        {
            SetCommands();
            LoadExtensions();
        }

        private void LoadExtensions()
        {
            IExtensionStorage extensionStorage = new DatabaseStorage();
            ExtensionsList = extensionStorage.ReadLastUsedExtensions();
        }

        private void OpenSettings(object parameter = null)
        {

        }

        private void UpdateExtension(object parameter = null)
        {
            // parameter = CommandParameter (ext id)
            if (parameter != null && parameter is string id)
            {
                IExtensionStorage storage = new DatabaseStorage();
                DataModels.Extension extension = storage.ReadExtension(id);
                _navigationService.Navigate("CrxSelectPage", extension);
            }
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
