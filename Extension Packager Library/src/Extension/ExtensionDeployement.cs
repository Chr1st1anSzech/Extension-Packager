// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager_Library.src.DataModels;
using System.IO;
using System.Threading.Tasks;

namespace Extension_Packager_Library.src.Extension
{
    internal interface IExtensionDeployement
    {
        internal Task DeployAsync(DeployementInfoData infos);
    }

    internal class ExtensionDeployement : ExtensionPostProcess, IExtensionDeployement
    {
        internal ExtensionDeployement(bool canOverwrite = false) : base(canOverwrite)
        {

        }

        public async Task DeployAsync(DeployementInfoData infos)
        {
            _log.Info("Deploy the extension.");

            CreateExtensionDirectory(infos.DeployementDirectory);
            await WriteXmlManifestAsync(infos.XmlManifest, infos.XmlManifestName, infos.DeployementDirectory);
            string destinationFile = Path.Combine(infos.DeployementDirectory, infos.CrxName);
            CopyFile(destinationFile, infos.CrxFile);
        }
    }
}
