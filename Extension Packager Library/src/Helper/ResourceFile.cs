// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;
using System.Reflection;

namespace Extension_Packager_Library.src.Helper
{
    public class ResourceFile
    {
        public static bool TryGetFullPath(string fileName, out string path)
        {
            path = GetFullPath(fileName);
            return File.Exists(path);
        }


        public static string GetFullPath(string fileName)
        {
            return Path.Combine(GetApplicationRoot(), fileName);
        }

        private static string GetApplicationRoot()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
    }
}
