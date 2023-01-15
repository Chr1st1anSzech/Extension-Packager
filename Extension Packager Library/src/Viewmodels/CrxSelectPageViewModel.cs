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
        private string _crxFile = @"E:\Downloads\extension_1_45_2_0.crx";
#else
        private string _crxFile = string.Empty;
#endif
        public string CrxFile
        {
            get { return _crxFile; }
            set { SetField(ref _crxFile, value); }
        }

        private string _privateKeyFile = string.Empty;
        public string PrivateKeyFile
        {
            get { return _privateKeyFile; }
            set { SetField(ref _privateKeyFile, value); }
        }


        private bool _isCrxFileValide = true;
        public bool IsCrxFileValid
        {
            get { return _isCrxFileValide; }
            set { SetField(ref _isCrxFileValide, value); }

        }

        private bool _isPrivateKeyFileValide = true;
        public bool IsPrivateKeyFileValid
        {
            get { return _isPrivateKeyFileValide; }
            set { SetField(ref _isPrivateKeyFileValide, value); }

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
            PageParameter.IsPageBack = true;
            _navigationService.Navigate("MainPage");
        }

        private void GoForward()
        {
            PageParameter.IsPageBack = false;
            _navigationService.Navigate("ManifestEditPage", PageParameter);
        }

        public void Reset()
        {
            if (PageParameter.Extension.ExtensionWorkingDirectory == null) return;
            try
            {
                Directory.Delete(PageParameter.Extension.ExtensionWorkingDirectory, true);
            }
            catch (Exception exception)
            {
                ErrorMessage = StringResources.GetWithReason(this, 6, exception.Message);
            }
        }


        private async void ProcessAndContinue(object parameter = null)
        {
            IsBusy = true;
            if (!IsInputValid()) return;

            PageParameter.Extension ??= new()
            {
                TmpPrivateKeyFile = PrivateKeyFile
            };

            bool directoriesCreated = CreateDirectories();
            if (!directoriesCreated) return;

            bool isSuccess = await UnpackAsync(CrxFile, PageParameter.Extension.UnpackedCrxDirectory);
            if (!isSuccess) return;

            Manifest manifest = ReadManifest(PageParameter.Extension.UnpackedCrxDirectory);
            if (manifest == null) return;

            SetExtensionValues(manifest);
            GoForward();
        }

        private bool IsInputValid()
        {
            if (!InputValidator.IsValidCrxFile(CrxFile))
            {
                IsBusy = false;
                ErrorMessage = StringResources.Get(this, 1);
                ErrorOccurred = true;
                return false;
            }
            if (PageParameter.IsAddition && !InputValidator.IsValidFile(PrivateKeyFile, ".pem"))
            {
                IsBusy = false;
                ErrorMessage = StringResources.Get(this, 7);
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

                PageParameter.Extension.ExtensionWorkingDirectory = workingDirectory;
                PageParameter.Extension.UnpackedCrxDirectory = unpackedCrxDirectory;

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
            PageParameter.Extension.ManifestContent = manifest.RawContent;
            PageParameter.Extension.Name = PageParameter.Extension.Name ?? manifest.Name;
            PageParameter.Extension.Version = manifest.Version;
            PageParameter.Extension.ShortName = PageParameter.Extension.ShortName ?? new ShortNameFormatter().Format(manifest.Name);
            PageParameter.Extension.ManifestFile = manifest.File;
        }
    }
}
