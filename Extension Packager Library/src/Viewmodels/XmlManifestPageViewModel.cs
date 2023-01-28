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
using System.IO;

namespace Extension_Packager_Library.src.Viewmodels
{
    public class XmlManifestPageViewModel : ViewModel
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
            AppId = PageParameter.Extension.Id;
        }


        private void SetCommands()
        {
            GoBackCommand = new MyCommand(GoBack);
            ProcessAndContinueCommand = new MyCommand(ProcessAndContinue);
            PreviewXmlManifestCommand = new MyCommand(PreviewXmlManifest);
        }


        private void GoBack(object parameter = null)
        {
            PageParameter.IsPageBack = true;
            _navigationService.Navigate("ManifestEditPage", PageParameter);
        }

        private void GoForward()
        {
            PageParameter.IsPageBack = false;
            _navigationService.Navigate("OptionalInfosPage", PageParameter);
        }


        private async void ProcessAndContinue(object parameter = null)
        {
            IsBusy = true;
            string xmlManifest = CreateXmlManifest();
            if (xmlManifest == null) return;
            PageParameter.Set("XmlManifestContent", xmlManifest);

            if (!await DeployExtensionAsync()) { return; }
            if (!await BackupExtensionAsync()) { return; }

            if (!StoreExtension()) { return; }

            string policyString = CreatePolicyString(PageParameter.Extension.ShortName);
            if (policyString == null) return;
            PageParameter.Set("PolicyString", policyString);

            DeleteTemporaryFiles();

            IsBusy = false;

            GoForward();
        }

        private void DeleteTemporaryFiles()
        {
            if (PageParameter.Get<string>("ExtensionWorkingDirectory") == null) return;
            try
            {
                Directory.Delete(PageParameter.Get<string>("ExtensionWorkingDirectory"), true);
            }
            catch (Exception ex)
            {
                ShowWarning(StringResources.GetWithReason(this, 5, ex.Message), ex);
            }
        }

        private bool StoreExtension()
        {
            IExtensionStorage storage = new DatabaseStorage();
            storage.Set(PageParameter.Extension);
            storage.SetLastModified(PageParameter.Extension);
            return true;
        }

        private async Task<bool> DeployExtensionAsync()
        {
            try
            {
                DataModels.Settings settings = SettingsRepository.Instance.ReadSettings();
                string extDeployementDir = Path.Combine(settings.DeployementDirectory, PageParameter.Extension.ShortName);
                Constants.ExtInOutputDir ExtInOutputDir = PageParameter.Get<Constants.ExtInOutputDir>("ExtInOutputDir");
                if (ExtInOutputDir != Constants.ExtInOutputDir.Full)
                {
                    DeployementInfoData deployementInfos = new()
                    {
                        DeployementDirectory = extDeployementDir,
                        CrxFile = PageParameter.Get<string>("TmpPackedCrxFile"),
                        CrxName = settings.CrxName,
                        XmlManifest = PageParameter.Get<string>("XmlManifestContent"),
                        XmlManifestName = settings.XmlManifestName
                    };

                    bool canOverwrite = PageParameter.IsUpdate || ExtInOutputDir == Constants.ExtInOutputDir.Partial;
                    IExtensionDeployement deployment = new ExtensionDeployement(canOverwrite);
                    await deployment.DeployAsync(deployementInfos);
                }
                PageParameter.Extension.DeployementDir = extDeployementDir;
                return true;
            }
            catch (Exception ex)
            {
                ShowWarning(StringResources.GetWithReason(this, 1, ex.Message), ex);
                return false;
            }
        }

        private async Task<bool> BackupExtensionAsync()
        {
            try
            {
                DataModels.Settings settings = SettingsRepository.Instance.ReadSettings();
                string extBackupDeployementDir = Path.Combine(settings.BackupDirectory, PageParameter.Extension.ShortName);
                Constants.ExtInOutputDir ExtInOutputDir = PageParameter.Get<Constants.ExtInOutputDir>("ExtInOutputDir");
                if (ExtInOutputDir != Constants.ExtInOutputDir.Full)
                {
                    BackupInfoData backupInfos = new()
                    {
                        BackupDirectory = extBackupDeployementDir,
                        CrxFile = PageParameter.Get<string>("TmpPackedCrxFile"),
                        CrxName = settings.CrxName,
                        PrivateKeyName = settings.PrivateKeyName,
                        XmlManifest = PageParameter.Get<string>("XmlManifestContent"),
                        XmlManifestName = settings.XmlManifestName,
                        TmpPrivateKeyPath = PageParameter.Get<string>("PrivateKeyFile")
                    };
                    bool canOverwrite = PageParameter.IsUpdate || ExtInOutputDir == Constants.ExtInOutputDir.Partial;
                    IExtensionBackup backup = new ExtensionBackup(canOverwrite);
                    await backup.BackupAsync(backupInfos);
                }
                PageParameter.Extension.BackupDir = extBackupDeployementDir;
                return true;
            }
            catch (Exception ex)
            {
                ShowWarning(StringResources.GetWithReason(this, 2, ex.Message), ex);
                return false;
            }
        }

        private string CreatePolicyString(string name)
        {
            if (name is null)
            {
                ShowWarning(StringResources.Get(this, 4));
                return null;
            }

            DataModels.Settings settings = SettingsRepository.Instance.ReadSettings();
            string xmlManifestFileUrl = Uri.Combine(settings.OutputURL, name, settings.XmlManifestName);
            return new PolicyStringGenerator().Create(PageParameter.Extension.Id, xmlManifestFileUrl);
        }

        private string CreateXmlManifest()
        {
            XmlManifest xmlManifest = new();
            DataModels.Settings settings = SettingsRepository.Instance.ReadSettings();
            string crxFileUrl = Uri.Combine(settings.OutputURL, PageParameter.Extension.ShortName, settings.CrxName);
            try
            {
                return xmlManifest.Create(crxFileUrl, PageParameter.Extension.Version, PageParameter.Extension.Id);
            }
            catch (Exception ex)
            {
                ShowWarning(StringResources.GetWithReason(this, 3, ex.Message), ex);
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
