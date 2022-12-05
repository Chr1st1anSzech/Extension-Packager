// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager_Library.src.Settings;
using log4net;
using System.Reflection;

namespace Extension_Packager.src.Startup
{
    class Startup
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void SetupSettingsRepo(ISettingsReader reader, ISettingsWriter writer)
        {
            SettingsRepository.Initialize(reader, writer);
        }

        public bool AreSettingsValide()
        {
            return new SettingsCheck().AreSettingsValide(SettingsRepository.Instance.ReadSettings());
        }
    }
}
