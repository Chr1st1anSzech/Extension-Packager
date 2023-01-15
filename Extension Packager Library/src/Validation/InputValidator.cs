// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager_Library.src.Extension;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Extension_Packager_Library.src.Validation
{
    public class InputValidator
    {
        public static bool IsNameValid(string name)
        {
            return Regex.IsMatch(name, "^.{1,64}$");
        }

        public static bool IsShortNameValide(string shortName)
        {
            return Regex.IsMatch(shortName, "^[A-Za-z_\\-0-9]{1,64}$");
        }

        public static bool IsValidFile(string file, string fileExtension)
        {
            if (string.IsNullOrWhiteSpace(file)) return false;

            if (fileExtension[0] != '.') fileExtension = $".{fileExtension}";

            FileInfo fileInfo = new(file);
            return fileInfo.Exists && fileInfo.Extension.Equals(fileExtension);
        }

        public static bool IsValidCrxFile(string file)
        {
            if (!IsValidFile(file, ".crx")) return false;
            try
            {
                byte[] buf = File.ReadAllBytes(file);
                CrxInfo crxInfo = new(buf);
                return crxInfo.MagicNumber == "Cr24";
            }
            catch
            {
                return false;
            }
        }
    }
}
