// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager_Library.src.DataModels;
using System.IO;
using System.Threading.Tasks;

namespace Extension_Packager_Library.src.Extension
{
    public interface IExtensionBackup
    {
        public Task<(string, string)> BackupAsync(BackupInfoData infos);
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
        public async Task<(string, string)> BackupAsync(BackupInfoData infos)
        {
            _log.Info("Backup the extension and private key.");

            string extensionDirectory = CreateExtensionDirectory(infos.BackupDirectory, infos.Name);

            string xmlManifestFile = await WriteXmlManifestAsync(infos.XmlManifest, infos.XmlManifestName, extensionDirectory);

            string crxFile = Path.Combine(extensionDirectory, infos.CrxName);
            CopyFile(crxFile, infos.CrxPath);

            string privateKeyFile = Path.Combine(extensionDirectory, infos.PrivateKeyName);
            if (File.Exists(infos.TmpPrivateKeyPath))
            {
                CopyFile(privateKeyFile, infos.TmpPrivateKeyPath);
            }

            return (xmlManifestFile, privateKeyFile);
        }
    }
}
