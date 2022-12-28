// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager.src.Helpers;
using Extension_Packager_Library.src.DataModels;
using Extension_Packager_Library.src.Extension;
using Extension_Packager_Library.src.Helper;
using Extension_Packager_Library.src.Navigation;
using Extension_Packager_Library.src.Settings;
using Extension_Packager_Library.src.Validation;
using log4net;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Extension_Packager_Library.src.Viewmodels
{
    public class CrxSelectPageViewModel : ViewModel
    {

        #region Public Properties

        private bool _isStepBack;
        public bool IsStepBack
        {
            get { return _isStepBack; }
            set { SetField(ref _isStepBack, value); }
        }

        private INavigationService _navigationService;
        public INavigationService NavigationService
        {
            get { return _navigationService; }
            set { SetField(ref _navigationService, value); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetField(ref _isBusy, value); }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { SetField(ref _errorMessage, value); }
        }

        private bool _errorOccurred;
        public bool ErrorOccurred
        {
            get { return _errorOccurred; }
            set { SetField(ref _errorOccurred, value); }
        }

#if DEBUG
        private string _extensionPath = @"E:\Downloads\extension_1_45_2_0.crx";
#else
        private string _extensionPath = string.Empty;
#endif
        public string ExtensionPath
        {
            get { return _extensionPath; }
            set { SetField(ref _extensionPath, value); }
        }

        #endregion


        #region Private Fields

        private DataModels.Extension _ext = ExtensionManager.Instance.CurrentExtension;

        #endregion


        #region Static Fields
        
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion


        #region Commands


        public MyCommand ProcessAndContinueCommand { get; set; }
        public MyCommand GoBackCommand { get; set; }



        private void SetCommands()
        {
            ProcessAndContinueCommand = new MyCommand(ProcessAndContinue);
            GoBackCommand = new MyCommand(GoBack);
        }


        #endregion



        public CrxSelectPageViewModel()
        {
            SetCommands();
        }


        private void GoBack(object parameter = null)
        {
            _navigationService.Navigate("MainPage");
        }


        private async void ProcessAndContinue(object parameter = null)
        {
            IsBusy = true;
            if (!IsInputValide()) return;

            _ext = new();
            ExtensionManager.Instance.CurrentExtension = _ext;
            await UnpackAndReadExtensionAsync();
            //IsBusy = false;
            _navigationService.Navigate("ManifestEditPage");
        }

        private bool IsInputValide()
        {
            if (!InputValidator.IsCrxPathValide(ExtensionPath))
            {
                IsBusy = false;
                ErrorMessage = StringResources.Get(this, 1);
                _log.Warn($"No CRX file was selected or the selected file \"{ExtensionPath}\" was not found.");
                ErrorOccurred = true;
                return false;
            }
            ErrorOccurred = false;
            return true;
        }

        private async Task UnpackAndReadExtensionAsync()
        {
            (string, string) directories = CreateDirectories();
            if (directories.Item1 == null || directories.Item2 == null) return;

            _ext.ExtensionWorkingDirectory = directories.Item1;
            _ext.UnpackedCrxDirectory = directories.Item2;

            bool isSuccess = await UnpackExtensionAsync(ExtensionPath, _ext.UnpackedCrxDirectory);
            if (!isSuccess) return;

            Manifest manifest = ReadManifest(_ext.UnpackedCrxDirectory);
            if (manifest == null) return;

            SetExtensionValues(manifest);
        }

        private (string, string) CreateDirectories()
        {
            DataModels.Settings settings = SettingsRepository.Instance.ReadSettings();
            try
            {
                string workingDirectory = FileHelper.CreateRandomDirectory(settings.WorkingAreaPath);
                string unpackedCrxDirectory = FileHelper.CreateRandomDirectory(workingDirectory);
                return (workingDirectory, unpackedCrxDirectory);
            }
            catch (Exception exception)
            {
                ErrorMessage = StringResources.GetWithReason(this, 4, exception.Message);
                ErrorOccurred = true;
                _log.Error(exception);
                return (null, null);
            }
        }

        private async Task<bool> UnpackExtensionAsync(string sourceCrxFile, string unpackedCrxDirectory)
        {
            IExtensionDepackager depackager = new ExtensionDepackager();
            try
            {
                await depackager.UnpackCrxAsync(sourceCrxFile, unpackedCrxDirectory);
                return true;
            }
            catch (Exception exception)
            {
                ErrorMessage = StringResources.GetWithReason(this, 5, exception.Message);
                ErrorOccurred = true;
                _log.Error(exception);
                return false;
            }
        }

        private Manifest ReadManifest(string unpackedCrxDirectory)
        {
            IManifestReader manifestReader = new ManifestReader(unpackedCrxDirectory);
            try
            {
                Manifest manifest = manifestReader.ReadManifest();
                return manifest;
            }
            catch (Exception exception)
            {
                ErrorMessage = StringResources.GetWithReason(this, 3, exception.Message);
                ErrorOccurred = true;
                _log.Warn(exception);
                return null;
            }
        }

        private void SetExtensionValues(Manifest manifest)
        {
            _ext.ManifestContent = manifest.RawContent;
            _ext.Name = manifest.Name;
            _ext.Version = manifest.Version;
            _ext.ShortName = new ShortNameFormatter().Format(manifest.Name);
            _ext.ManifestFile = manifest.File;
        }
    }
}
