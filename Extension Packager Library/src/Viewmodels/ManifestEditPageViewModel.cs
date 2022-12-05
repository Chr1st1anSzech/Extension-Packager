// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager.src.Helpers;
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

        private readonly DataModels.Extension _ext = ExtensionManager.Instance.CurrentExtension;

        #endregion


        #region Static Fields

        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion


        #region Commands


        public MyCommand NextPageCommand { get; set; }
        public MyCommand CancelCommand { get; set; }
        public MyCommand ManifestPreviewCommand { get; set; }
        public MyCommand ResetValuesCommand { get; set; }


        private void SetCommands()
        {
            NextPageCommand = new MyCommand(ProcessAndContinue);
            CancelCommand = new MyCommand(Cancel);
            ManifestPreviewCommand = new MyCommand(ShowManifest);
            ResetValuesCommand = new MyCommand(ResetValues);
        }


        #endregion


        public ManifestEditPageViewModel()
        {
            SetCommands();
            SetProperties();
        }


        private void SetProperties()
        {
            Name = _ext.Name;
            ShortName = _ext.ShortName;
            Version = _ext.Version;
        }


        private void Cancel(object parameter = null)
        {

        }


        private void ResetValues(object parameter = null)
        {
            Name = _ext.Name;
            ShortName = _ext.ShortName;
        }


        private async void ProcessAndContinue(object parameter = null)
        {
            IsBusy = true;
            if (!IsInputValide()) return;

            SetExtensionValues();

            if (!ChangeManifest()) return;

            if (!await SaveManifestAsync()) return;

            string packedCryFile = PackExtension();
            if (packedCryFile == null) return;
            _ext.PackedCrxFile = packedCryFile;

            string privateKeyFile = FindPrivateKeyFile(_ext.ExtensionWorkingDirectory);
            if (privateKeyFile == null) return;
            _ext.PrivateKeyFile = privateKeyFile;

            string appId = ExtractAppId(_ext.PackedCrxFile);
            if (appId == null) return;
            _ext.Id = appId;

            IsBusy = false;
            _navigationService.Navigate("XmlManifestPage");
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
            _ext.Name = Name;
            _ext.ShortName = ShortName;
        }


        private bool ChangeManifest()
        {
            DataModels.Settings settings = SettingsRepository.Instance.ReadSettings();
            string updateUrl = Helper.Uri.Combine(settings.OutputURL, ShortName, settings.XmlManifestName);
            try
            {
                _ext.ManifestContent = Manifest.ChangeUpdateUrl(_ext.ManifestContent, updateUrl);
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
                await File.WriteAllTextAsync(_ext.ManifestFile, _ext.ManifestContent);
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


        private string PackExtension()
        {
            try
            {
                DataModels.Settings settings = SettingsRepository.Instance.ReadSettings();
                IExtensionPacker packer = new ExtensionPacker();
                return packer.Pack(settings.BrowserPath, settings.ExtensionPathParameter, _ext.UnpackedCrxDirectory);
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

            ExtensionManifestDocument.SetText(TextSetOptions.FormatRtf, _ext.ManifestContent);

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
