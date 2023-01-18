// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager_Library.src.Database;
using Extension_Packager_Library.src.DataModels;
using Extension_Packager_Library.src.Extension;
using Extension_Packager_Library.src.Formatter;
using Extension_Packager_Library.src.Helper;
using Extension_Packager_Library.src.Navigation;
using Extension_Packager_Library.src.Settings;
using Extension_Packager_Library.src.Validation;
using log4net;
using Microsoft.UI.Text;
using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Extension_Packager_Library.src.Viewmodels
{
    public class ManifestEditPageViewModel : ViewModel
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

        private bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetField(ref _isBusy, value); }

        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { SetField(ref _errorMessage, value); }

        }


        private bool _errorOccured = false;
        public bool ErrorOccured
        {
            get { return _errorOccured; }
            set { SetField(ref _errorOccured, value); }

        }


        private bool _isEditBoxReadOnly = true;
        public bool IsEditBoxReadOnly
        {
            get { return _isEditBoxReadOnly; }
            set { SetField(ref _isEditBoxReadOnly, value); }

        }

        private bool _isNameValide = true;
        public bool IsNameValid
        {
            get { return _isNameValide; }
            set { SetField(ref _isNameValide, value); }

        }

        private bool _isShortNameValide = true;
        public bool IsShortNameValid
        {
            get { return _isShortNameValide; }
            set { SetField(ref _isShortNameValide, value); }

        }


        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set
            {
                SetField(ref _name, value);
                ShortName = new ShortNameFormatter().Format(value);
            }
        }


        private string _version = string.Empty;
        public string Version
        {
            get { return _version; }
            set
            {
                SetField(ref _version, value);
            }
        }


        private string _shortName = string.Empty;
        public string ShortName
        {
            get { return _shortName; }
            set
            {
                SetField(ref _shortName, value);
            }
        }


        private RichEditTextDocument _extensionManifestDocument;
        public RichEditTextDocument ExtensionManifestDocument
        {
            get { return _extensionManifestDocument; }
            set
            {
                SetField(ref _extensionManifestDocument, value);
            }
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
        public MyCommand ManifestPreviewCommand { get; set; }
        public MyCommand ResetValuesCommand { get; set; }


        private void SetCommands()
        {
            ProcessAndContinueCommand = new MyCommand(ProcessAndContinue);
            GoBackCommand = new MyCommand(GoBack);
            ManifestPreviewCommand = new MyCommand(ShowManifest);
            ResetValuesCommand = new MyCommand(SetProperties);
        }


        #endregion


        public void Init()
        {
            SetCommands();
            SetProperties();
        }


        private void GoBack(object parameter = null)
        {
            PageParameter.IsPageBack = true;
            _navigationService.Navigate("CrxSelectPage", PageParameter);
        }

        private void GoForward()
        {
            PageParameter.IsPageBack = false;
            _navigationService.Navigate("XmlManifestPage", PageParameter);
        }

        public void Reset()
        {
            if (PageParameter.Get<string>("TmpPackedCrxFile") == null) return;
            try
            {
                File.Delete(PageParameter.Get<string>("TmpPackedCrxFile"));
                if (PageParameter.Get<string>("PrivateKeyFile") != null)
                {
                    File.Delete(PageParameter.Get<string>("PrivateKeyFile"));
                }
            }
            catch (Exception exception)
            {
                ErrorMessage = StringResources.GetWithReason(this, 9, exception.Message);
            }
        }


        private void SetProperties(object parameter = null)
        {
            Name = PageParameter.Extension.Name ?? string.Empty;
            ShortName = PageParameter.Extension.ShortName ?? string.Empty;
            Version = PageParameter.Extension.Version ?? string.Empty;
        }


        private async void ProcessAndContinue(object parameter = null)
        {
            IsBusy = true;
            if (!IsInputValide()) return;

            SetExtensionValues();

            if (!ChangeManifest()) return;

            if (!await SaveManifestAsync()) return;

            if (PageParameter.IsUpdate)
            {
                string privateKeyFile = FindPrivateKeyFile(PageParameter.Extension.BackupDir);
                if (privateKeyFile == null) return;
                PageParameter.Set("PrivateKeyFile", privateKeyFile);
            }

            string packedCrxFile = PackExtension(PageParameter.Get<string>("UnpackedCrxDirectory"), PageParameter.Get<string>("PrivateKeyFile"));
            if (packedCrxFile == null) return;
            PageParameter.Set("TmpPackedCrxFile", packedCrxFile);

            if (!PageParameter.IsUpdate && !PageParameter.IsAddition)
            {
                string tmpPrivateKeyFile = FindPrivateKeyFile(PageParameter.Get<string>("ExtensionWorkingDirectory"));
                if (tmpPrivateKeyFile == null) return;
                PageParameter.Set("PrivateKeyFile", tmpPrivateKeyFile);
            }

            string appId = PageParameter.Extension.Id ?? ExtractAppId(PageParameter.Get<string>("PrivateKeyFile"));
            if (appId == null) return;
            PageParameter.Extension.Id = appId;

            IsBusy = false;

            GoForward();
        }


        private bool IsInputValide()
        {
            if (!InputValidator.IsNameValid(Name))
            {
                IsNameValid = false;
                IsBusy = false;
                ErrorMessage = StringResources.Get(this, 7);
                ErrorOccured = true;
                return false;
            }

            if (!InputValidator.IsShortNameValide(ShortName))
            {
                IsShortNameValid = false;
                IsBusy = false;
                ErrorMessage = StringResources.Get(this, 8);
                ErrorOccured = true;
                return false;
            }

            if(IsDuplicateShortname())
            {
                IsShortNameValid = false;
                IsBusy = false;
                ErrorMessage = StringResources.Get(this, 10);
                ErrorOccured = true;
                return false;
            }

            IsNameValid = true;
            IsShortNameValid = true;
            ErrorOccured = false;
            return true;
        }

        private bool IsDuplicateShortname()
        {
            IExtensionStorage storage = new DatabaseStorage();
            int count = storage.GetCountByShortname(ShortName);
            return count > 0;
        }


        private void SetExtensionValues()
        {
            PageParameter.Extension.Name = Name;
            PageParameter.Extension.ShortName = ShortName;
        }


        private bool ChangeManifest()
        {
            DataModels.Settings settings = SettingsRepository.Instance.ReadSettings();
            string updateUrl = Helper.Uri.Combine(settings.OutputURL, ShortName, settings.XmlManifestName);
            try
            {
                PageParameter.Set("ManifestContent", Manifest.ChangeUpdateUrl(PageParameter.Get<string>("ManifestContent"), updateUrl));
            }
            catch (Exception ex)
            {
                IsBusy = false;
                ErrorMessage = StringResources.GetWithReason(this, 3, ex.Message);
                ErrorOccured = true;
                _log.Error(ex);
                return false;
            }
            return true;
        }


        private async Task<bool> SaveManifestAsync()
        {
            try
            {
                await File.WriteAllTextAsync(PageParameter.Get<string>("ManifestFile"), PageParameter.Get<string>("ManifestContent"));
                return true;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                ErrorMessage = StringResources.GetWithReason(this, 1, ex.Message);
                ErrorOccured = true;
                _log.Error(ex);
                return false;
            }
        }


        /// <summary>
        /// Packs an unpacked extension.
        /// </summary>
        /// <returns>Path to the newly packed extension</returns>
        private string PackExtension(string unpackedCrxDirectory, string privateKeyFile)
        {
            try
            {
                DataModels.Settings settings = SettingsRepository.Instance.ReadSettings();
                IExtensionPacker packer = new ExtensionPacker();
                return packer.Pack(settings.BrowserPath,
                    settings.ExtensionPathParameter, unpackedCrxDirectory,
                    settings.ExtensionKeyParameter, privateKeyFile);
            }
            catch (Exception ex)
            {
                IsBusy = false;
                ErrorMessage = StringResources.GetWithReason(this, 2, ex.Message);
                ErrorOccured = true;
                _log.Error(ex);
            }
            return null;
        }


        private string FindPrivateKeyFile(string searchDirectory)
        {
            string[] files = FileHelper.FindFiles(searchDirectory, ".pem");

            if (files.Length != 1)
            {
                IsBusy = false;
                ErrorMessage = StringResources.Get(this, 6);
                ErrorOccured = true;
                return null;
            }

            return files[0];
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
                ErrorMessage = StringResources.GetWithReason(this, 4, ex.Message);
                ErrorOccured = true;
                _log.Error(ex);
                return null;
            }
        }


        private void ShowManifest(object parameter = null)
        {
            IsEditBoxReadOnly = false;

            if (!IsInputValide())
            {
                ExtensionManifestDocument.SetText(TextSetOptions.FormatRtf, string.Empty);
                IsEditBoxReadOnly = true;
                return;
            }

            ChangeManifest();

            ExtensionManifestDocument.SetText(TextSetOptions.FormatRtf, PageParameter.Get<string>("ManifestContent"));

            Formatter.Formatter formatter = new JSONFormatter()
            {
                //KeyColor = App.Current.Resources["KeyColor"] as Color? ?? Color.FromArgb(255, 120, 120, 120),
                //BoolNullColor = App.Current.Resources["BoolNullColor"] as Color? ?? Color.FromArgb(255, 120, 120, 120),
                //NumberColor = App.Current.Resources["NumberColor"] as Color? ?? Color.FromArgb(255, 120, 120, 120),
                //StringColor = App.Current.Resources["StringColor"] as Color? ?? Color.FromArgb(255, 120, 120, 120),
                //HighlightColor = App.Current.Resources["HighlightColor"] as Color? ?? Color.FromArgb(120, 120, 120, 120)
            };
            formatter.SyntaxHighlight(ExtensionManifestDocument);
            IsEditBoxReadOnly = true;
        }
    }
}
