// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager_Library.src.DataModels;
using Extension_Packager_Library.src.Extension;
using Extension_Packager_Library.src.Helper;
using Extension_Packager_Library.src.Navigation;
using Extension_Packager_Library.src.Settings;
using Extension_Packager_Library.src.Validation;
using log4net;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Extension_Packager_Library.src.Viewmodels
{
    public class CrxSelectPageViewModel : ViewModel
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

            PageParameter param = new()
            {
                Extension = Extension,
                IsUpdate = IsUpdate
            };
            _navigationService.Navigate("MainPage", param);
        }

        private void GoForward()
        {
            PageParameter param = new()
            {
                Extension = Extension,
                IsUpdate = IsUpdate
            };
            _navigationService.Navigate("ManifestEditPage", param);
        }

        public void Reset()
        {
            if (Extension.ExtensionWorkingDirectory == null) return;
            try
            {
                Directory.Delete(Extension.ExtensionWorkingDirectory, true);
            }
            catch (Exception exception)
            {
                ErrorMessage = StringResources.GetWithReason(this, 6, exception.Message);
            }
        }


        private async void ProcessAndContinue(object parameter = null)
        {
            IsBusy = true;
            if (!IsInputValide()) return;

            Extension ??= new();

            bool directoriesCreated = CreateDirectories();
            if (!directoriesCreated) return;

            bool isSuccess = await UnpackAsync(ExtensionPath, Extension.UnpackedCrxDirectory);
            if (!isSuccess) return;

            Manifest manifest = ReadManifest(Extension.UnpackedCrxDirectory);
            if (manifest == null) return;

            SetExtensionValues(manifest);
            GoForward();
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



        private bool CreateDirectories()
        {
            DataModels.Settings settings = SettingsRepository.Instance.ReadSettings();
            try
            {
                string workingDirectory = FileHelper.CreateRandomDirectory(settings.WorkingAreaPath);
                string unpackedCrxDirectory = FileHelper.CreateRandomDirectory(workingDirectory);

                Extension.ExtensionWorkingDirectory = workingDirectory;
                Extension.UnpackedCrxDirectory = unpackedCrxDirectory;

                return true;
            }
            catch (Exception exception)
            {
                ErrorMessage = StringResources.GetWithReason(this, 4, exception.Message);
                ErrorOccurred = true;
                _log.Error(exception);

                return false;
            }
        }



        private async Task<bool> UnpackAsync(string sourceCrxFile, string unpackedCrxDirectory)
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
            Extension.ManifestContent = manifest.RawContent;
            Extension.Name = Extension.Name ?? manifest.Name;
            Extension.Version = manifest.Version;
            Extension.ShortName = Extension.ShortName ?? new ShortNameFormatter().Format(manifest.Name);
            Extension.ManifestFile = manifest.File;
        }
    }
}
