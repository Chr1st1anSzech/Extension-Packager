// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager_Library.src.DataModels;
using System.IO;
using System.Threading.Tasks;

namespace Extension_Packager_Library.src.Extension
{
    public interface IExtensionBackup
    {
        public Task BackupAsync(BackupInfoData infos);
    }


    public class ExtensionBackup : ExtensionPostProcess, IExtensionBackup
    {
        public ExtensionBackup(bool canOverwrite = false) : base(canOverwrite)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infos"></param>
        /// <returns></returns>
        public async Task BackupAsync(BackupInfoData infos)
        {
            _log.Info("Backup the extension and private key.");

            CreateExtensionDirectory(infos.BackupDirectory);

            await WriteXmlManifestAsync(infos.XmlManifest, infos.XmlManifestName, infos.BackupDirectory);

            string crxFile = Path.Combine(infos.BackupDirectory, infos.CrxName);
            CopyFile(crxFile, infos.CrxFile);

            string privateKeyFile = Path.Combine(infos.BackupDirectory, infos.PrivateKeyName);
            if (!_canOverwrite && File.Exists(infos.TmpPrivateKeyPath))
            {
                CopyFile(privateKeyFile, infos.TmpPrivateKeyPath);
            }
        }
    }
}
