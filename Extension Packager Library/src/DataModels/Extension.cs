// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Extension_Packager_Library.src.DataModels
{
    public class Extension
    {
        public string Name { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string ManifestContent { get; set; } = string.Empty;
        public string XmlManifest { get; set; } = string.Empty;
        public string PolicyString { get; set; } = string.Empty;

        public string PackedCrxFile { get; set; } = string.Empty;
        public string PrivateKeyFile { get; set; } = string.Empty;
        public string UnpackedCrxDirectory { get; set; } = string.Empty;
        public string ManifestFile { get; set; } = string.Empty;
        public string ExtensionWorkingDirectory { get; set; } = string.Empty;

    }
}
