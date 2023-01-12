// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager_Library.src.Database;
using Extension_Packager_Library.src.DataModels;
using Extension_Packager_Library.src.Helper;
using Extension_Packager_Library.src.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Extension_Packager_Library.src.Viewmodels
{
    public class MainPageViewModel : ViewModel
    {
        #region Public Properties

        public List<string> BrowserFilterList { get; } = new() { "Alle", "Google Chrome", "Microsoft Edge" };

        private List<DataModels.Extension> _lastModifiedExtensions = new();
        public List<DataModels.Extension> LastModifiedExtensions
        {
            get { return _lastModifiedExtensions; }
            set { SetField(ref _lastModifiedExtensions, value); }
        }


        private INavigationService _navigationService;
        public INavigationService NavigationService
        {
            get { return _navigationService; }
            set { SetField(ref _navigationService, value); }
        }

        #endregion

        #region Private Fields

        private List<DataModels.Extension> _notFilteredLastModifiedExtensions;

        #endregion

        #region Commands

        public MyCommand SettingsCommand { get; set; }
        public MyCommand CreateExtensionCommand { get; set; }
        public MyCommand ShowExtensionsListCommand { get; set; }
        public MyCommand UpdateExtensionCommand { get; set; }
        public MyCommand SearchCommand { get; set; }

        private void SetCommands()
        {
            SettingsCommand = new MyCommand(OpenSettings);
            CreateExtensionCommand = new MyCommand(CreateExtension);
            ShowExtensionsListCommand = new MyCommand(ShowExtensionsList);
            UpdateExtensionCommand = new MyCommand(UpdateExtension);
            SearchCommand = new MyCommand(Search);
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
            LastModifiedExtensions = extensionStorage.GetAllLastModified();
        }

        private void OpenSettings(object parameter = null)
        {

        }

        private void Search(object parameter = null)
        {
            if (parameter is string searchText && searchText != null)
            {
                _notFilteredLastModifiedExtensions ??= LastModifiedExtensions;

                if (searchText.Equals(string.Empty))
                {
                    LastModifiedExtensions = _notFilteredLastModifiedExtensions;
                    return;
                }

                LastModifiedExtensions = _notFilteredLastModifiedExtensions.Where(extension =>
                {
                    return extension.Name.ToLower().Contains(searchText.ToLower());
                }).ToList();
            }
        }

        private void UpdateExtension(object parameter = null)
        {
            // parameter = CommandParameter (ext id)
            if (parameter != null && parameter is string id)
            {
                IExtensionStorage storage = new DatabaseStorage();
                PageParameter param = new()
                {
                    Extension = storage.Get(id),
                    IsUpdate = true
                };
                _navigationService.Navigate("CrxSelectPage", param);
            }
        }

        private void CreateExtension(object parameter = null)
        {
            _navigationService.Navigate("CrxSelectPage");
        }

        private void ShowExtensionsList(object parameter = null)
        {
            _navigationService.Navigate("ExtensionListPage");
        }
    }
}
