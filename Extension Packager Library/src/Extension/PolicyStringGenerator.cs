// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Extension_Packager_Library.src.Extension
{
    public class PolicyStringGenerator
    {
        public string Create(string extensionId, string xmlManifestUrl)
        {
            return $"{extensionId};{xmlManifestUrl}";
        }
    }
}
