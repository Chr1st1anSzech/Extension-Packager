// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Extension_Packager_Library.src.DataModels
{
    public class Extension
    {
        /// <summary>
        /// Full name of the extension.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Shortened name of the extension. Used for the directory path.
        /// </summary>
        public string ShortName { get; set; } = string.Empty;

        /// <summary>
        /// ID of the extension.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Version of the extension.
        /// </summary>
        public string Version { get; set; } = string.Empty;

        /// <summary>
        /// Content of the manifest file. The file is in the packaged extension.
        /// </summary>
        public string ManifestContent { get; set; } = string.Empty;

        /// <summary>
        /// Content of the XML manifest file. It is located in the extension directory and refers to the CRX file.
        /// </summary>
        public string XmlManifest { get; set; } = string.Empty;

        /// <summary>
        /// Character string that can be used for the group policy.
        /// </summary>
        public string PolicyString { get; set; } = string.Empty;

        /// <summary>
        /// Path to the packed extension
        /// </summary>
        public string PackedCrxFile { get; set; } = string.Empty;

        /// <summary>
        /// Path to the key file
        /// </summary>
        public string PrivateKeyFile { get; set; } = string.Empty;

        /// <summary>
        /// Path of the unpacked extension
        /// </summary>
        public string UnpackedCrxDirectory { get; set; } = string.Empty;

        /// <summary>
        /// Path of the manifest file in the directory of the unpacked extension.
        /// </summary>
        public string ManifestFile { get; set; } = string.Empty;

        /// <summary>
        /// Working directory in which the extension is unpacked and packaged.
        /// </summary>
        public string ExtensionWorkingDirectory { get; set; } = string.Empty;

    }
}
