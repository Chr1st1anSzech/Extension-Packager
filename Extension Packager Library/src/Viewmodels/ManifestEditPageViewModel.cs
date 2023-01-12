// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
using System.Threading.Tasks;

namespace Extension_Packager_Library.src.Viewmodels
{
    public class ManifestEditPageViewModel : ViewModel
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
            PageParameter param = new()
            {
                Extension = Extension,
                IsUpdate = IsUpdate,
                IsPageBack = true
            };
            _navigationService.Navigate("CrxSelectPage", param);
        }

        private void GoForward()
        {
            PageParameter param = new()
            {
                Extension = Extension,
                IsUpdate = IsUpdate,
                IsPageBack = false
            };
            _navigationService.Navigate("XmlManifestPage", param);
        }

        public void Reset()
        {
            if (Extension.TmpPackedCrxFile == null) return;
            try
            {
                File.Delete(Extension.TmpPackedCrxFile);
                if (Extension.TmpPrivateKeyFile != null)
                {
                    File.Delete(Extension.TmpPrivateKeyFile);
                }
            }
            catch (Exception exception)
            {
                ErrorMessage = StringResources.GetWithReason(this, 9, exception.Message);
            }
        }


        private void SetProperties(object parameter = null)
        {
            Name = Extension.Name ?? string.Empty;
            ShortName = Extension.ShortName ?? string.Empty;
            Version = Extension.Version ?? string.Empty;
        }


        private async void ProcessAndContinue(object parameter = null)
        {
            IsBusy = true;
            if (!IsInputValide()) return;

            SetExtensionValues();

            if (!ChangeManifest()) return;

            if (!await SaveManifestAsync()) return;

            string packedCrxFile = PackExtension();
            if (packedCrxFile == null) return;
            Extension.TmpPackedCrxFile = packedCrxFile;

            if (!IsUpdate)
            {
                string privateKeyFile = FindPrivateKeyFile(Extension.ExtensionWorkingDirectory);
                if (privateKeyFile == null) return;
                Extension.TmpPrivateKeyFile = privateKeyFile;
            }

            string appId = Extension.Id ?? ExtractAppId(Extension.TmpPackedCrxFile);
            if (appId == null) return;
            Extension.Id = appId;

            IsBusy = false;

            GoForward();
        }


        private bool IsInputValide()
        {
            if (!InputValidator.IsNameValide(Name))
            {
                IsBusy = false;
                ErrorMessage = StringResources.Get(this, 7);
                ErrorOccured = true;
                return false;
            }

            if (!InputValidator.IsShortNameValide(ShortName))
            {
                IsBusy = false;
                ErrorMessage = StringResources.Get(this, 8);
                ErrorOccured = true;
                return false;
            }
            ErrorOccured = false;
            return true;
        }


        private void SetExtensionValues()
        {
            Extension.Name = Name;
            Extension.ShortName = ShortName;
        }


        private bool ChangeManifest()
        {
            DataModels.Settings settings = SettingsRepository.Instance.ReadSettings();
            string updateUrl = Helper.Uri.Combine(settings.OutputURL, ShortName, settings.XmlManifestName);
            try
            {
                Extension.ManifestContent = Manifest.ChangeUpdateUrl(Extension.ManifestContent, updateUrl);
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
                await File.WriteAllTextAsync(Extension.ManifestFile, Extension.ManifestContent);
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
        private string PackExtension()
        {
            try
            {
                DataModels.Settings settings = SettingsRepository.Instance.ReadSettings();
                IExtensionPacker packer = new ExtensionPacker();
                return packer.Pack(settings.BrowserPath, 
                    settings.ExtensionPathParameter, Extension.UnpackedCrxDirectory,
                    settings.ExtensionKeyParameter, Extension.PrivateKeyFile);
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
            string[] pemFiles;
            try
            {
                pemFiles = Directory.GetFiles(searchDirectory, "*.pem");
            }
            catch (Exception ex)
            {
                IsBusy = false;
                ErrorMessage = StringResources.GetWithReason(this, 5,ex.Message);
                ErrorOccured = true;
                _log.Error(ex);
                return null;
            }

            if (pemFiles.Length != 1)
            {
                IsBusy = false;
                ErrorMessage = StringResources.Get(this, 6);
                ErrorOccured = true;
                return null;
            }
            return pemFiles[0];
        }


        private string ExtractAppId(string outputPath)
        {
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                throw new ArgumentException($"\"{nameof(outputPath)}\" should not be NULL or whitespace.", nameof(outputPath));
            }

            IAppIdReader appIdReader = new AppIdReader();
            try
            {
                return appIdReader.GetAppId(outputPath);
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

            ExtensionManifestDocument.SetText(TextSetOptions.FormatRtf, Extension.ManifestContent);

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
