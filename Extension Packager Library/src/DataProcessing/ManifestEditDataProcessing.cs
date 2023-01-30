using Extension_Packager_Library.src.DataModels;
using Extension_Packager_Library.src.Extension;
using Extension_Packager_Library.src.Helper;
using Extension_Packager_Library.src.Settings;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Extension_Packager_Library.src.DataProcessing
{
    internal class ManifestEditDataProcessing
    {
        #region Private Fields

        private readonly Action<string, Exception> _showWarning;

        private readonly PageParameter _pageParameter;
        private readonly string _shortName;

        #endregion


        #region Static Fields

        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        public ManifestEditDataProcessing(string shortName, PageParameter pageParameter, Action<string, Exception> ShowWarning)
        {
            _shortName = shortName;
            _pageParameter = pageParameter;
            _showWarning = ShowWarning;
        }

        public async Task<bool> ProcessInput()
        {
            if (!ChangeManifest(_shortName)) return false;

            if (!await SaveManifestAsync()) return false;

            if (_pageParameter.IsUpdate)
            {
                string privateKeyFile = FindPrivateKeyFile(_pageParameter.Extension.BackupDir);
                if (privateKeyFile == null) return false;
                _pageParameter.Set("PrivateKeyFile", privateKeyFile);
            }


            string packedCrxFile = PackExtension(_pageParameter.Get<string>("UnpackedCrxDirectory"), _pageParameter.Get<string>("PrivateKeyFile"));
            if (packedCrxFile == null) return false;
            _pageParameter.Set("TmpPackedCrxFile", packedCrxFile);


            if (!_pageParameter.IsUpdate && !_pageParameter.IsAddition)
            {
                string tmpPrivateKeyFile = FindPrivateKeyFile(_pageParameter.Get<string>("ExtensionWorkingDirectory"));
                if (tmpPrivateKeyFile == null) return false;
                _pageParameter.Set("PrivateKeyFile", tmpPrivateKeyFile);
            }

            string appId = _pageParameter.Extension.Id ?? ExtractAppId(_pageParameter.Get<string>("PrivateKeyFile"));
            if (appId == null) return false;
            _pageParameter.Extension.Id = appId;

            return true;
        }


        public bool ChangeManifest(string shortName)
        {
            DataModels.Settings settings = SettingsRepository.Instance.ReadSettings();
            string updateUrl = Helper.Uri.Combine(settings.OutputURL, shortName, settings.XmlManifestName);
            try
            {
                _pageParameter.Set("ManifestContent", 
                    Manifest.ChangeUpdateUrl(_pageParameter.Get<string>("ManifestContent"), updateUrl));
            }
            catch (Exception ex)
            {
                _showWarning(StringResources.GetWithReason(this, 3, ex.Message), ex);
                return false;
            }
            return true;
        }


        private async Task<bool> SaveManifestAsync()
        {
            try
            {
                await File.WriteAllTextAsync(_pageParameter.Get<string>("ManifestFile"), _pageParameter.Get<string>("ManifestContent"));
                return true;
            }
            catch (Exception ex)
            {
                _showWarning(StringResources.GetWithReason(this, 1, ex.Message), ex);
                return false;
            }
        }


        private string FindPrivateKeyFile(string searchDirectory)
        {
            string[] files = FileHelper.FindFiles(searchDirectory, ".pem");

            if (files.Length != 1)
            {
                _showWarning(StringResources.Get(this, 6), null);
                return null;
            }

            return files[0];
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
                _showWarning(StringResources.GetWithReason(this, 2, ex.Message), ex);
            }
            return null;
        }


        private string ExtractAppId(string privateKeyFile)
        {
            if (string.IsNullOrWhiteSpace(privateKeyFile))
            {
                _showWarning($"\"{nameof(privateKeyFile)}\" should not be NULL or whitespace.", null);
                return null;
            }

            IAppIdReader appIdReader = new AppIdReader();
            try
            {
                return appIdReader.GetAppIdByPrivateKey(privateKeyFile);
            }
            catch (Exception ex)
            {
                _showWarning(StringResources.GetWithReason(this, 4, ex.Message), ex);
                return null;
            }
        }
    }
}
