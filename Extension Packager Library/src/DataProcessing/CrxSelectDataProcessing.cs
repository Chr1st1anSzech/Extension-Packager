using Extension_Packager_Library.src.DataModels;
using Extension_Packager_Library.src.Extension;
using Extension_Packager_Library.src.Helper;
using Extension_Packager_Library.src.Settings;
using log4net;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Extension_Packager_Library.src.DataProcessing
{
    internal class CrxSelectDataProcessing
    {

        #region Private Fields

        private readonly Action<string, Exception> _showWarning;

        private readonly string _crxFile;
        private readonly string _privateKeyFile;
        private readonly PageParameter _pageParameter;

        #endregion


        #region Static Fields

        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion


        internal CrxSelectDataProcessing(string crxFile, string privateKeyFile,
            PageParameter pageParameter, Action<string, Exception> ShowWarning)
        {
            _crxFile = crxFile;
            _privateKeyFile = privateKeyFile;
            _pageParameter = pageParameter;
            _showWarning = ShowWarning;
        }

        internal async Task<Manifest> ProcessInput()
        {
            bool successfullCheck = CheckForExtInOutputDir();
            if (!successfullCheck) return null;

            bool directoriesCreated = CreateDirectories();
            if (!directoriesCreated) return null;

            string unpackedCrxDirectory = _pageParameter.Get<string>("UnpackedCrxDirectory");

            bool isSuccess = await UnpackAsync(_crxFile, unpackedCrxDirectory);
            if (!isSuccess) return null;

            Manifest manifest = ReadManifest(unpackedCrxDirectory);
            return manifest;
        }

        private bool CheckForExtInOutputDir()
        {
            DataModels.Settings settings = SettingsReaderFactory.Create().ReadSettings();
            try
            {
                bool isCrxInDeployementDir = IsFileInSubdirOf(_crxFile, settings.DeployementDirectory, out string subDirectory1);
                bool isPrivateKeyInBackupDir = IsFileInSubdirOf(_privateKeyFile, settings.BackupDirectory, out string subDirectory2);
                if (isCrxInDeployementDir)
                {
                    _pageParameter.Set("Shortname1", subDirectory1);
                }
                if (isPrivateKeyInBackupDir)
                {
                    _pageParameter.Set("Shortname2", subDirectory2);
                }
                return true;
            }
            catch (Exception exception)
            {
                _showWarning(StringResources.GetWithReason(this, 1, exception.Message), exception);
                return false;
            }
        }


        private bool IsFileInSubdirOf(string file, string compareDirectory, out string subDir)
        {
            if (!File.Exists(file) || !Directory.Exists(compareDirectory))
            {
                subDir = null;
                return false;
            }

            DirectoryInfo parentDir = new FileInfo(file).Directory;
            subDir = parentDir.Name;
            DirectoryInfo secondParentDir = parentDir.Parent;
            return secondParentDir.Parent != null &&
                secondParentDir.FullName == Path.GetFullPath(compareDirectory);
        }


        private bool CreateDirectories()
        {
            DataModels.Settings settings = SettingsRepository.Instance.ReadSettings();
            try
            {
                string workingDirectory = FileHelper.CreateRandomDirectory(settings.WorkingAreaPath);
                string unpackedCrxDirectory = FileHelper.CreateRandomDirectory(workingDirectory);

                _pageParameter.Set("ExtensionWorkingDirectory", workingDirectory);
                _pageParameter.Set("UnpackedCrxDirectory", unpackedCrxDirectory);

                return true;
            }
            catch (Exception exception)
            {
                _showWarning(StringResources.GetWithReason(this, 2, exception.Message), exception);
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
                _showWarning(StringResources.GetWithReason(this, 3, exception.Message), exception);
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
                _showWarning(StringResources.GetWithReason(this, 4, exception.Message), exception);
                return null;
            }
        }
    }
}
