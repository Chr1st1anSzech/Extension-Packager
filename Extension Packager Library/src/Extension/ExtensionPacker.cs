// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Diagnostics;

namespace Extension_Packager_Library.src.Extension
{
    public interface IExtensionPacker
    {
        /// <summary>
        /// Packs an unpacked extension by calling the browser.
        /// </summary>
        /// <param name="browserPath">Path to the browser executable.</param>
        /// <param name="pathParam">Parameter name for the directory path of the unpacked extension</param>
        /// <param name="extensionPath">Directory path of the unpacked extension</param>
        /// <param name="keyParam">Parameter name for the key file</param>
        /// <param name="keyPath">Path of the key file</param>
        /// <returns>Path to the newly packed extension</returns>
        public string Pack(string browserPath, string pathParam, string extensionPath, string keyParam = null, string keyPath = null);
    }

    public class ExtensionPacker : IExtensionPacker
    {
        /// <summary>
        /// Packs an unpacked extension by calling the browser.
        /// </summary>
        /// <param name="browserPath">Path to the browser executable.</param>
        /// <param name="pathParam">Parameter name for the directory path of the unpacked extension</param>
        /// <param name="extensionPath">Directory path of the unpacked extension</param>
        /// <param name="keyParam">Parameter name for the key file</param>
        /// <param name="keyPath">Path of the key file</param>
        /// <returns>Path to the newly packed extension</returns>
        public string Pack(string browserPath, string pathParam, string extensionPath, string keyParam = null, string keyPath = null)
        {
            string privateKeyParameter = (string.IsNullOrWhiteSpace(keyParam)
                || string.IsNullOrWhiteSpace(keyPath)) ? "" : $"{keyParam}=\"{keyPath}\"";

            Process p = new();
            p.StartInfo.FileName = browserPath;
            p.StartInfo.Arguments = $"{pathParam}=\"{extensionPath}\" {privateKeyParameter}";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            p.WaitForExit(10000);
            return extensionPath + ".crx";
        }
    }
}
