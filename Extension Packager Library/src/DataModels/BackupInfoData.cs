﻿// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Extension_Packager_Library.src.DataModels
{
    internal class BackupInfoData
    {
        internal string BackupDirectory { get; set; }
        internal string TmpPrivateKeyPath { get; set; }
        internal string PrivateKeyName { get; set; }
        internal string XmlManifest { get; set; }
        internal string XmlManifestName { get; set; }
        internal string CrxName { get; set; }
        internal string CrxFile { get; set; }
    }
}
