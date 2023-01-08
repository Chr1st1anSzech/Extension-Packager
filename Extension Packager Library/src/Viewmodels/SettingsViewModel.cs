// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager_Library.src.Helper;
using Extension_Packager_Library.src.Navigation;
using Extension_Packager_Library.src.Settings;

namespace Extension_Packager_Library.src.Viewmodels
{
    public class SettingsViewModel : ViewModel
    {
        #region Public Properties

        private INavigationService _navigationService;
        public INavigationService NavigationService
        {
            get { return _navigationService; }
            set { SetField(ref _navigationService, value); }
        }

        public bool AreDefaultSettings { get; set; }
        public DataModels.Settings Settings { get; set; }

        #endregion


        #region Commands

        public MyCommand ConfirmSettingsCommand { get; set; }

        private void SetCommands()
        {
            ConfirmSettingsCommand = new MyCommand(ConfirmSettings);
        }

        #endregion



        public SettingsViewModel()
        {
            SetCommands();
            LoadSettings();
            ShowDefaultSettingsMessage();
        }

        private void LoadSettings()
        {
            Settings = SettingsRepository.Instance.ReadSettings();
        }

        private void ShowDefaultSettingsMessage()
        {
            if (!Settings.IsFirstRun && SettingsRepository.Instance.AreDefaultSettings)
            {
                AreDefaultSettings = true;
            }
        }

        private void ConfirmSettings(object parameter = null)
        {
            Settings.IsFirstRun = false;
            SettingsRepository.Instance.AreDefaultSettings = false;
            ISettingsWriter writer = SettingsWriterFactory.Create();
            writer.WriteSettings(Settings);
            _navigationService.Navigate("MainPage");
        }
    }
}
