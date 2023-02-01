// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager_Library.src.Database;
using Extension_Packager_Library.src.DataModels;
using Extension_Packager_Library.src.DataProcessing;
using Extension_Packager_Library.src.Extension;
using Extension_Packager_Library.src.Helper;
using Extension_Packager_Library.src.Navigation;
using Extension_Packager_Library.src.Validation;
using log4net;
using System;
using System.IO;
using System.Reflection;

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
            if (PageParameter.Get<string>("ExtensionWorkingDirectory") == null) return;
            try
            {
                Directory.Delete(PageParameter.Get<string>("ExtensionWorkingDirectory"), true);
            }
            catch (Exception exception)
            {
                WarningMessage = StringResources.GetWithReason(this, 6, exception.Message);
            }
        }


        private async void ProcessAndContinue(object parameter = null)
        {
            IsBusy = true;
            if (!IsInputValid()) return;

            PageParameter.Extension ??= new();
            PageParameter.Set("PrivateKeyFile", PrivateKeyFile);

            CrxSelectDataProcessing dataProcessing = new(CrxFile, PrivateKeyFile, PageParameter, ShowWarning);

            Manifest manifest = await dataProcessing.ProcessInput();
            if (manifest == null) return;

            SetExtensionValues(manifest);

            IsBusy = false;
            GoForward();
        }


        private bool IsInputValid()
        {
            bool areAllInputsValid = IsValidOrWarn(() =>
            {
                return InputValidator.IsValidCrxFile(CrxFile);
            }, StringResources.Get(this, 1));


            areAllInputsValid &= IsValidOrWarn(() =>
            {
                return !PageParameter.IsAddition || InputValidator.IsValidFile(PrivateKeyFile, ".pem");
            }, StringResources.Get(this, 7));


            if (PageParameter.IsAddition)
            {
                string appId = ExtractAppId(PrivateKeyFile);
                areAllInputsValid &= IsValidOrWarn(() =>
                {
                    string appId = ExtractAppId(PrivateKeyFile);
                    return IdAlreadyExists(appId);

                }, StringResources.Get(this, 8, appId));
            }

            if (!areAllInputsValid) return false;

            IsWarningVisible = false;
            return true;
        }


        private void SetExtensionValues(Manifest manifest)
        {
            PageParameter.Extension.Name = PageParameter.Extension.Name ?? manifest.Name;
            PageParameter.Extension.ShortName = PageParameter.Extension.ShortName ?? new ShortNameFormatter().Format(manifest.Name);
            PageParameter.Extension.Version = manifest.Version;
            PageParameter.Set("ManifestContent", manifest.RawContent);
            PageParameter.Set("ManifestFile", manifest.File);
        }


        private string ExtractAppId(string privateKeyFile)
        {
            if (string.IsNullOrWhiteSpace(privateKeyFile))
            {
                throw new ArgumentException($"\"{nameof(privateKeyFile)}\" should not be NULL or whitespace.", nameof(privateKeyFile));
            }

            IAppIdReader appIdReader = new AppIdReader();
            try
            {
                return appIdReader.GetAppIdByPrivateKey(privateKeyFile);
            }
            catch (Exception ex)
            {
                IsBusy = false;
                //ErrorMessage = StringResources.GetWithReason(this, 4, ex.Message);
                IsWarningVisible = true;
                _log.Error(ex);
                return null;
            }
        }


        private bool IdAlreadyExists(string id)
        {
            if (id == null) return false;

            IExtensionStorage storage = new DatabaseStorage();
            int count = storage.GetCountById(id);
            return count > 0;
        }

    }
}
