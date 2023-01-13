// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager_Library.src.DataModels;
using System.IO;
using System.Threading.Tasks;

namespace Extension_Packager_Library.src.Extension
{
    public interface IExtensionBackup
    {
        public Task<string> BackupAsync(BackupInfoData infos);
    }


    public class ExtensionBackup : ExtensionPostProcess, IExtensionBackup
    {
        public ExtensionBackup(bool isUpdate = false) : base(isUpdate)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infos"></param>
        /// <returns></returns>
        public async Task<string> BackupAsync(BackupInfoData infos)
        {
            _log.Info("Backup the extension and private key.");

            string backupDirectory = CreateExtensionDirectory(infos.BackupDirectory, infos.Name);

            string xmlManifestFile = await WriteXmlManifestAsync(infos.XmlManifest, infos.XmlManifestName, backupDirectory);

            string crxFile = Path.Combine(backupDirectory, infos.CrxName);
            CopyFile(crxFile, infos.CrxPath);

            string privateKeyFile = Path.Combine(backupDirectory, infos.PrivateKeyName);
            if (File.Exists(infos.TmpPrivateKeyPath))
            {
                CopyFile(privateKeyFile, infos.TmpPrivateKeyPath);
            }

            return backupDirectory;
        }
    }
}
