// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Extension_Packager_Library.src.Extension
{
    internal class PolicyStringGenerator
    {
        internal string Create(string extensionId, string xmlManifestUrl)
        {
            return $"{extensionId};{xmlManifestUrl}";
        }
    }
}
