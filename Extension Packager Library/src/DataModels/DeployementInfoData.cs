// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Extension_Packager_Library.src.DataModels
{
    public class DeployementInfoData
    {
        public string DestinationDirectory { get; set; }
        public string XmlManifest { get; set; }
        public string XmlManifestName { get; set; }
        public string CrxName { get; set; }
        public string CrxPath { get; set; }
        public string Name { get; set; }
    }
}
