// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Extension_Packager_Library.src.Settings
{
    public class SettingsCheck : ISettingsCheck
    {
        public bool AreSettingsValide(DataModels.Settings settings)
        {
            bool areParametersValide = !string.IsNullOrWhiteSpace(settings.ExtensionKeyParameter) && !string.IsNullOrWhiteSpace(settings.ExtensionPathParameter);
            bool isAppXmlValide = !string.IsNullOrWhiteSpace(settings.XmlManifestName) &&
                !settings.XmlManifestName.Any(Path.GetInvalidFileNameChars().Contains);
            bool arePathsValide = Directory.Exists(settings.WorkingAreaPath) && File.Exists(settings.BrowserPath) && Directory.Exists(settings.DeployementDirectory);
            bool isWebsiteValide = Regex.IsMatch(settings.OutputURL, "[(http(s)?):\\/\\/(www\\.)?a-zA-Z0-9@:%._\\+~#=]{2,256}\\.[a-z]{2,6}\\b([-a-zA-Z0-9@:%_\\+.~#?&//=]*)");
            return areParametersValide && isAppXmlValide && arePathsValide && isWebsiteValide;
        }
    }
}
