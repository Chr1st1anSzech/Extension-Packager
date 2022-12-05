// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager_Library.src.Helper;
using System.IO;

namespace Extension_Packager_Library.src.Settings
{
    public static class SettingsFile
    {
        private static readonly string _filename = "config.json";

        public static bool TryGetValidePath(out string path)
        {
            path = ResourceFile.GetFullPath(_filename);
            return File.Exists(path);
        }
    }
}
