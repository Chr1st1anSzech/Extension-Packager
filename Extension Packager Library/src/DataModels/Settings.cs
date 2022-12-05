// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;

namespace Extension_Packager_Library.src.DataModels
{
    public class Settings
    {
        public string BrowserPath { get; set; } = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";
        public string WorkingAreaPath { get; set; } = Path.GetTempPath();
        public string ExtensionPathParameter { get; set; } = "--pack-extension";
        public string ExtensionKeyParameter { get; set; } = "--pack-extension-key";
        public string OutputPath { get; set; } = @"C:\";
        public string OutputURL { get; set; } = @"https://my-server.com/Extensions/";
        public string XmlManifestName { get; set; } = "ext.xml";
        public string CrxName { get; set; } = "ext.crx";
        public string PrivateKeyName { get; set; } = "ext.pem";
        public bool IsFirstRun { get; set; } = true;
        public string BackupDirectory { get; set; } = @"C:\";
    }
}
