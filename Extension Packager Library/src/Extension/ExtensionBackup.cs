// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager_Library.src.DataModels;
using System.IO;
using System.Threading.Tasks;

namespace Extension_Packager_Library.src.Extension
{
    public interface IExtensionBackup
    {
        public Task<(string,string)> BackupAsync(BackupInfoData infos);
    }


    public class ExtensionBackup : ExtensionPostProcess, IExtensionBackup
    {
        public ExtensionBackup(bool isUpdate = false) : base(isUpdate)
        {

        }

        public async Task<(string, string)> BackupAsync(BackupInfoData infos)
        {
            _log.Info("Backup the extension and private key.");

            string extensionDirectory = CreateExtensionDirectory(infos.BackupDirectory, infos.Name);
            await WriteXmlManifestAsync(infos.XmlManifest, infos.XmlManifestName, extensionDirectory);

            string destinationFile1 = Path.Combine(extensionDirectory, infos.CrxName);
            CopyFile(destinationFile1, infos.CrxPath);

            string destinationFile2 = Path.Combine(extensionDirectory, infos.PrivateKeyName);
            CopyFile(destinationFile2, infos.PrivateKeyPath);

            return (destinationFile1, destinationFile2);
        }
    }
}
