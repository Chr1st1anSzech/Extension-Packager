// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Extension_Packager_Library.src.DataModels
{
    public class DeployementInfoData
    {
        public string DeployementDirectory { get; set; }
        public string XmlManifest { get; set; }
        public string XmlManifestName { get; set; }
        public string CrxName { get; set; }
        public string CrxFile { get; set; }
    }
}
