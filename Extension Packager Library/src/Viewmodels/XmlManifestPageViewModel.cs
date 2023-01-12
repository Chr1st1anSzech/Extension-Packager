// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.UI.Text;
using Extension_Packager_Library.src.DataModels;
using Extension_Packager_Library.src.Extension;
using Extension_Packager_Library.src.Settings;
using Extension_Packager_Library.src.Formatter;
using System.Threading.Tasks;
using Extension_Packager_Library.src.Helper;
using log4net;
using System;
using System.Reflection;
using Uri = Extension_Packager_Library.src.Helper.Uri;
using Extension_Packager_Library.src.Navigation;
using Extension_Packager_Library.src.Database;

namespace Extension_Packager_Library.src.Viewmodels
{
    public class XmlManifestPageViewModel : ViewModel
    {
        #region Public Properties

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

        private string _appId = string.Empty;
        public string AppId
        {
            get { return _appId; }
            set { SetField(ref _appId, value); }

        }


        private RichEditTextDocument _extensionXmlManifestDocument;
        public RichEditTextDocument ExtensionXmlManifestDocument
        {
            get { return _extensionXmlManifestDocument; }
            set
            {
                SetField(ref _extensionXmlManifestDocument, value);
            }
        }

        #endregion


        #region Private Fields

        #endregion


        #region Static Fields

        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion


        #region Commands

        public MyCommand GoBackCommand { get; set; }
        public MyCommand ProcessAndContinueCommand { get; set; }
        public MyCommand PreviewXmlManifestCommand { get; set; }

        #endregion


        public void Init()
        {
            SetCommands();
            SetProperties();
        }


        private void SetProperties()
        {
            AppId = Extension.Id;
        }


        private void SetCommands()
        {
            GoBackCommand = new MyCommand(GoBack);
            ProcessAndContinueCommand = new MyCommand(ProcessAndContinue);
            PreviewXmlManifestCommand = new MyCommand(PreviewXmlManifest);
        }


        private void GoBack(object parameter = null)
        {
            PageParameter param = new()
            {
                Extension = Extension,
                IsUpdate = IsUpdate
            };
            _navigationService.Navigate("ManifestEditPage", param);
        }

        private void GoForward()
        {
            PageParameter param = new()
            {
                Extension = Extension,
                IsUpdate = IsUpdate
            };
            _navigationService.Navigate("SuccessPage", param);
        }


        private async void ProcessAndContinue(object parameter = null)
        {
            IsBusy = true;
            string xmlManifest = CreateXmlManifest();
            if (xmlManifest == null) return;
            Extension.XmlManifestContent = xmlManifest;

            if (!await DeployExtensionAsync()) { return; }
            if(!await BackupExtensionAsync()) { return; }

            if (!StoreExtension()) { return; }

            string policyString = CreatePolicyString(Extension.ShortName);
            if (policyString == null) return;
            Extension.PolicyString = policyString;

            IsBusy = false;

            GoForward();
        }

        private bool StoreExtension()
        {
            IExtensionStorage storage = new DatabaseStorage();
            storage.Set(Extension);
            storage.SetLastModified(Extension);
            return true;
        }

        private async Task<bool> DeployExtensionAsync()
        {
            DataModels.Settings settings = SettingsRepository.Instance.ReadSettings();
            DeployementInfoData deployementInfos = new()
            {
                CrxName = settings.CrxName,
                Name = Extension.ShortName,
                XmlManifest = Extension.XmlManifestContent,
                XmlManifestName = settings.XmlManifestName,
                DestinationDirectory = settings.OutputPath,
                CrxPath = Extension.TmpPackedCrxFile
            };
            IExtensionDeployement deployment = new ExtensionDeployement(IsUpdate);

            try
            {
                await deployment.DeployAsync(deployementInfos);
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


        private async Task<bool> BackupExtensionAsync()
        {
            DataModels.Settings settings = SettingsRepository.Instance.ReadSettings();
            BackupInfoData backupInfos = new()
            {
                BackupDirectory = settings.BackupDirectory,
                Name = Extension.ShortName,
                CrxPath = Extension.TmpPackedCrxFile,
                CrxName = settings.CrxName,
                XmlManifest = Extension.XmlManifestContent,
                XmlManifestName = settings.XmlManifestName,
                PrivateKeyPath = Extension.TmpPrivateKeyFile,
                PrivateKeyName = settings.PrivateKeyName
            };
            IExtensionBackup backup = new ExtensionBackup(IsUpdate);
            try
            {
                (string,string) destinationFiles = await backup.BackupAsync(backupInfos);
                Extension.XmlManifestFile = destinationFiles.Item1;
                Extension.PrivateKeyFile = destinationFiles.Item2;

                return true;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                ErrorMessage = StringResources.GetWithReason(this, 2, ex.Message);
                ErrorOccured = true;
                _log.Error(ex);
                return false;
            }
        }

        private string CreatePolicyString(string name)
        {
            if (name is null)
            {
                IsBusy = false;
                ErrorMessage = StringResources.Get(this, 4);
                ErrorOccured = true;
                return null;
            }

            DataModels.Settings settings = SettingsRepository.Instance.ReadSettings();
            string xmlManifestFileUrl = Uri.Combine(settings.OutputURL, name, settings.XmlManifestName);
            return new PolicyStringGenerator().Create(Extension.Id, xmlManifestFileUrl);
        }

        private string CreateXmlManifest()
        {
            XmlManifest xmlManifest = new();
            DataModels.Settings settings = SettingsRepository.Instance.ReadSettings();
            string crxFileUrl = Uri.Combine(settings.OutputURL, Extension.ShortName, settings.CrxName);
            try
            {
                return xmlManifest.Create(crxFileUrl, Extension.Version, Extension.Id);
            }
            catch (Exception ex)
            {
                IsBusy = false;
                ErrorMessage = StringResources.GetWithReason(this, 3, ex.Message);
                ErrorOccured = true;
                _log.Error(ex);
                return null;
            }
        }


        private void PreviewXmlManifest(object parameter = null)
        {
            IsEditBoxReadOnly = false;

            
            string xmlManifest = CreateXmlManifest();
            if (xmlManifest == null) return;

            ExtensionXmlManifestDocument.SetText(TextSetOptions.FormatRtf, xmlManifest);

            Formatter.Formatter formatter = new XmlFormatter()
            {
                //AttributeColor = App.Current.Resources["KeyColor"] as Color? ?? Color.FromArgb(255, 120, 120, 120),
                //ElementNameColor = App.Current.Resources["BoolNullColor"] as Color? ?? Color.FromArgb(255, 120, 120, 120),
                //ValueColor = App.Current.Resources["StringColor"] as Color? ?? Color.FromArgb(255, 120, 120, 120),
                //HighlightColor = App.Current.Resources["HighlightColor"] as Color? ?? Color.FromArgb(120, 120, 120, 120)
            };
            formatter.SyntaxHighlight(ExtensionXmlManifestDocument);

            IsEditBoxReadOnly = true;
        }
    }
}
