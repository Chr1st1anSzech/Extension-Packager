// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Diagnostics;

namespace Extension_Packager_Library.src.Extension
{
    public interface IExtensionPacker
    {
        public string Pack(string browserPath, string pathParam, string extensionPath, string keyParam = null, string keyPath = null);
    }

    public class ExtensionPacker : IExtensionPacker
    {
        public string Pack(string browserPath, string pathParam, string extensionPath, string keyParam = null, string keyPath = null)
        {
            Process p = new();
            p.StartInfo.FileName = browserPath;
            p.StartInfo.Arguments = $"{pathParam}={extensionPath} {keyParam}={keyPath}";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            p.WaitForExit(10000);
            return extensionPath + ".crx";
        }
    }
}
