// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;
using System.Text.RegularExpressions;

namespace Extension_Packager_Library.src.Validation
{
    public class InputValidator
    {
        public static bool IsNameValide(string name)
        {
            return Regex.IsMatch(name, "^.{1,64}$");
        }

        public static bool IsShortNameValide(string shortName)
        {
            return Regex.IsMatch(shortName, "^[A-Za-z_\\-0-9]{1,64}$");
        }

        public static bool IsCrxPathValide(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;

            FileInfo file = new(path);
            return file.Exists && file.Extension.Equals(".crx");
        }
    }
}
