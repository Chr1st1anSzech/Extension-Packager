// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Extension_Packager_Library.src.DataModels
{
    public class Extension
    {
        /// <summary>
        /// Full name of the extension.
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Shortened name of the extension. Used for the directory path.
        /// </summary>
        public string ShortName { get; set; }


        /// <summary>
        /// ID of the extension.
        /// </summary>
        public string Id { get; set; }


        /// <summary>
        /// Version of the extension.
        /// </summary>
        public string Version { get; set; }


        /// <summary>
        /// Content of the manifest file. The file is in the packaged extension.
        /// </summary>
        public string ManifestContent { get; set; }


        /// <summary>
        /// Path of the manifest file in the directory of the unpacked extension.
        /// </summary>
        public string ManifestFile { get; set; }


        /// <summary>
        /// Content of the XML manifest file. It is located in the extension directory and refers to the CRX file.
        /// </summary>
        public string XmlManifestContent { get; set; }


        /// <summary>
        /// Character string that can be used for the group policy.
        /// </summary>
        public string PolicyString { get; set; }


        /// <summary>
        /// Path to the temporary packed extension.
        /// </summary>
        public string TmpPackedCrxFile { get; set; }


        /// <summary>
        /// Path to the temporary key file.
        /// </summary>
        public string PrivateKeyFile { get; set; }


        /// <summary>
        /// Path of the unpacked extension
        /// </summary>
        public string UnpackedCrxDirectory { get; set; }


        /// <summary>
        /// Working directory in which the extension is unpacked and packaged.
        /// </summary>
        public string ExtensionWorkingDirectory { get; set; }


        /// <summary>
        /// The date when the extension was created or updated.
        /// </summary>
        public DateTime UpdateDate { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string DeployementDir { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string BackupDir { get; set; }

    }
}
