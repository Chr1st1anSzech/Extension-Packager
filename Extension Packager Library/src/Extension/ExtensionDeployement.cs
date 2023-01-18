// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager_Library.src.DataModels;
using System.IO;
using System.Threading.Tasks;

namespace Extension_Packager_Library.src.Extension
{
    public interface IExtensionDeployement
    {
        public Task<string> DeployAsync(DeployementInfoData infos);
    }

    public class ExtensionDeployement : ExtensionPostProcess, IExtensionDeployement
    {
        public ExtensionDeployement(bool isUpdate = false) : base(isUpdate)
        {

        }

        public async Task<string> DeployAsync(DeployementInfoData infos)
        {
            _log.Info("Deploy the extension.");

            string deployementDirectory = CreateExtensionDirectory(infos.DeployementDirectory, infos.Name);
            await WriteXmlManifestAsync(infos.XmlManifest, infos.XmlManifestName, deployementDirectory);
            string destinationFile = Path.Combine(deployementDirectory, infos.CrxName);
            CopyFile(destinationFile, infos.CrxFile);
            return deployementDirectory;
        }
    }
}
