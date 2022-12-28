// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager.src.Helpers;
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

namespace Extension_Packager_Library.src.Viewmodels
{
    public class XmlManifestPageViewModel : ViewModel
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

        private readonly DataModels.Extension _ext = ExtensionManager.Instance.CurrentExtension;

        #endregion


        #region Static Fields

        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion


        #region Commands

        public MyCommand GoBackCommand { get; set; }
        public MyCommand ProcessAndContinueCommand { get; set; }
        public MyCommand PreviewXmlManifestCommand { get; set; }

        #endregion



        public XmlManifestPageViewModel()
        {
            SetCommands();
            SetProperties();
        }


        private void SetProperties()
        {
            AppId = _ext.Id;
        }


        private void SetCommands()
        {
            GoBackCommand = new MyCommand(GoBack);
            ProcessAndContinueCommand = new MyCommand(ProcessAndContinue);
            PreviewXmlManifestCommand = new MyCommand(PreviewXmlManifest);
        }


        private void GoBack(object parameter = null)
        {
            _navigationService.Navigate("ManifestEditPage", true);
        }


        private async void ProcessAndContinue(object parameter = null)
        {
            IsBusy = true;
            string xmlManifest = CreateXmlManifest();
            if (xmlManifest == null) return;
            _ext.XmlManifest = xmlManifest;

            if (!await DeployExtension()) { return; }
            if(!await BackupExtension()) { return; }
            
            string policyString = CreatePolicyString(_ext.ShortName);
            if (policyString == null) return;
            _ext.PolicyString = policyString;

            IsBusy = false;
            _navigationService.Navigate("SuccessPage");
        }


        private async Task<bool> DeployExtension()
        {
            DataModels.Settings settings = SettingsRepository.Instance.ReadSettings();
            DeployementInfoData deployementInfos = new()
            {
                CrxName = settings.CrxName,
                Name = _ext.ShortName,
                XmlManifest = _ext.XmlManifest,
                XmlManifestName = settings.XmlManifestName,
                DestinationDirectory = settings.OutputPath,
                CrxPath = _ext.PackedCrxFile
            };
            IExtensionDeployement deployment = new ExtensionDeployement();

            try
            {
                await deployment.Deploy(deployementInfos);
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


        private async Task<bool> BackupExtension()
        {
            DataModels.Settings settings = SettingsRepository.Instance.ReadSettings();
            BackupInfoData backupInfos = new()
            {
                BackupDirectory = settings.BackupDirectory,
                Name = _ext.ShortName,
                CrxPath = _ext.PackedCrxFile,
                CrxName = settings.CrxName,
                XmlManifest = _ext.XmlManifest,
                XmlManifestName = settings.XmlManifestName,
                PrivateKeyPath = _ext.PrivateKeyFile,
                PrivateKeyName = settings.PrivateKeyName
            };
            IExtensionBackup backup = new ExtensionBackup();
            try
            {
                await backup.Backup(backupInfos);
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
            return new PolicyStringGenerator().Create(_ext.Id, xmlManifestFileUrl);
        }

        private string CreateXmlManifest()
        {
            XmlManifest xmlManifest = new();
            DataModels.Settings settings = SettingsRepository.Instance.ReadSettings();
            string crxFileUrl = Uri.Combine(settings.OutputURL, _ext.ShortName, settings.CrxName);
            try
            {
                return xmlManifest.Create(crxFileUrl, _ext.Version, _ext.Id);
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
