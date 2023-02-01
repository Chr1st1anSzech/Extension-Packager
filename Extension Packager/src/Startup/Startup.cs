// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager_Library.src.Settings;
using log4net;
using System.Reflection;

namespace Extension_Packager.src.Startup
{
    internal class Startup
    {
        internal void SetupSettingsRepo(ISettingsReader reader, ISettingsWriter writer)
        {
            SettingsRepository.Initialize(reader, writer);
        }

        internal bool AreSettingsValide()
        {
            return new SettingsCheck().AreSettingsValide(SettingsRepository.Instance.ReadSettings());
        }
    }
}
