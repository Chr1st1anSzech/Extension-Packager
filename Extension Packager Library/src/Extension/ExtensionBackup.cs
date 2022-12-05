// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager_Library.src.DataModels;
using System.Threading.Tasks;

namespace Extension_Packager_Library.src.Extension
{
    public interface IExtensionBackup
    {
        public Task Backup(BackupInfoData infos);
    }


    public class ExtensionBackup : ExtensionPostProcess, IExtensionBackup
    {
        public ExtensionBackup(bool isUpdate = false) : base(isUpdate)
        {

        }

        public async Task Backup(BackupInfoData infos)
        {
            _log.Info("Backup the extension and private key.");

            string extensionDirectory = CreateExtensionDirectory(infos.BackupDirectory, infos.Name);
            await WriteXmlManifestAsync(infos.XmlManifest, infos.XmlManifestName, extensionDirectory);
            CopyFile(extensionDirectory, infos.CrxPath, infos.CrxName);
            CopyFile(extensionDirectory, infos.PrivateKeyPath, infos.PrivateKeyName);
        }
    }
}
