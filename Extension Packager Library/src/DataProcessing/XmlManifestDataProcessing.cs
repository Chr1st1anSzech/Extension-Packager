using Extension_Packager_Library.src.Database;
using Extension_Packager_Library.src.DataModels;
using Extension_Packager_Library.src.Extension;
using Extension_Packager_Library.src.Helper;
using Extension_Packager_Library.src.Settings;
using log4net;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Uri = Extension_Packager_Library.src.Helper.Uri;

namespace Extension_Packager_Library.src.DataProcessing
{
    internal class XmlManifestDataProcessing
    {
        #region Private Fields

        private readonly Action<string, Exception> _showWarning;

        private readonly PageParameter _pageParameter;

        #endregion


        #region Static Fields

        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion


        internal XmlManifestDataProcessing(PageParameter pageParameter, Action<string, Exception> ShowWarning)
        {
            _pageParameter = pageParameter;
            _showWarning = ShowWarning;
        }


        internal async Task<bool> ProcessInput()
        {
            string xmlManifest = CreateXmlManifest();
            if (xmlManifest == null) return false;
            _pageParameter.Set("XmlManifestContent", xmlManifest);

            if (!await DeployExtensionAsync()) { return false; }
            if (!await BackupExtensionAsync()) { return false; }

            if (!StoreExtension()) { return false; }

            string policyString = CreatePolicyString(_pageParameter.Extension.ShortName);
            if (policyString == null) return false;
            _pageParameter.Set("PolicyString", policyString);

            DeleteTemporaryFiles();

            return true;
        }


        internal string CreateXmlManifest()
        {
            XmlManifest xmlManifest = new();
            DataModels.Settings settings = SettingsRepository.Instance.ReadSettings();
            string crxFileUrl = Uri.Combine(settings.OutputURL, _pageParameter.Extension.ShortName, settings.CrxName);
            try
            {
                return xmlManifest.Create(crxFileUrl, _pageParameter.Extension.Version, _pageParameter.Extension.Id);
            }
            catch (Exception ex)
            {
                _showWarning(StringResources.GetWithReason(this, 1, ex.Message), ex);
                return null;
            }
        }


        private void DeleteTemporaryFiles()
        {
            if (_pageParameter.Get<string>("ExtensionWorkingDirectory") == null) return;
            try
            {
                Directory.Delete(_pageParameter.Get<string>("ExtensionWorkingDirectory"), true);
            }
            catch (Exception ex)
            {
                _showWarning(StringResources.GetWithReason(this, 2, ex.Message), ex);
            }
        }


        private bool StoreExtension()
        {
            IExtensionStorage storage = new DatabaseStorage();
            storage.Set(_pageParameter.Extension);
            storage.SetLastModified(_pageParameter.Extension);
            return true;
        }


        private async Task<bool> DeployExtensionAsync()
        {
            try
            {
                DataModels.Settings settings = SettingsRepository.Instance.ReadSettings();
                string extDeployementDir = Path.Combine(settings.DeployementDirectory, _pageParameter.Extension.ShortName);
                Constants.ExtInOutputDir ExtInOutputDir = _pageParameter.Get<Constants.ExtInOutputDir>("ExtInOutputDir");
                if (ExtInOutputDir != Constants.ExtInOutputDir.Full)
                {
                    DeployementInfoData deployementInfos = new()
                    {
                        DeployementDirectory = extDeployementDir,
                        CrxFile = _pageParameter.Get<string>("TmpPackedCrxFile"),
                        CrxName = settings.CrxName,
                        XmlManifest = _pageParameter.Get<string>("XmlManifestContent"),
                        XmlManifestName = settings.XmlManifestName
                    };

                    bool canOverwrite = _pageParameter.IsUpdate || ExtInOutputDir == Constants.ExtInOutputDir.Partial;
                    IExtensionDeployement deployment = new ExtensionDeployement(canOverwrite);
                    await deployment.DeployAsync(deployementInfos);
                }
                _pageParameter.Extension.DeployementDir = extDeployementDir;
                return true;
            }
            catch (Exception ex)
            {
                _showWarning(StringResources.GetWithReason(this, 3, ex.Message), ex);
                return false;
            }
        }


        private async Task<bool> BackupExtensionAsync()
        {
            try
            {
                DataModels.Settings settings = SettingsRepository.Instance.ReadSettings();
                string extBackupDeployementDir = Path.Combine(settings.BackupDirectory, _pageParameter.Extension.ShortName);
                Constants.ExtInOutputDir ExtInOutputDir = _pageParameter.Get<Constants.ExtInOutputDir>("ExtInOutputDir");
                if (ExtInOutputDir != Constants.ExtInOutputDir.Full)
                {
                    BackupInfoData backupInfos = new()
                    {
                        BackupDirectory = extBackupDeployementDir,
                        CrxFile = _pageParameter.Get<string>("TmpPackedCrxFile"),
                        CrxName = settings.CrxName,
                        PrivateKeyName = settings.PrivateKeyName,
                        XmlManifest = _pageParameter.Get<string>("XmlManifestContent"),
                        XmlManifestName = settings.XmlManifestName,
                        TmpPrivateKeyPath = _pageParameter.Get<string>("PrivateKeyFile")
                    };
                    bool canOverwrite = _pageParameter.IsUpdate || ExtInOutputDir == Constants.ExtInOutputDir.Partial;
                    IExtensionBackup backup = new ExtensionBackup(canOverwrite);
                    await backup.BackupAsync(backupInfos);
                }
                _pageParameter.Extension.BackupDir = extBackupDeployementDir;
                return true;
            }
            catch (Exception ex)
            {
                _showWarning(StringResources.GetWithReason(this, 4, ex.Message), ex);
                return false;
            }
        }


        private string CreatePolicyString(string name)
        {
            if (name is null)
            {
                _showWarning(StringResources.Get(this, 5), null);
                return null;
            }

            DataModels.Settings settings = SettingsRepository.Instance.ReadSettings();
            string xmlManifestFileUrl = Uri.Combine(settings.OutputURL, name, settings.XmlManifestName);
            return new PolicyStringGenerator().Create(_pageParameter.Extension.Id, xmlManifestFileUrl);
        }

    }
}
