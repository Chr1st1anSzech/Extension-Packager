// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Extension_Packager_Library.src.Settings
{
    public class DefaultSettingReader : ISettingsReader
    {
        public DataModels.Settings ReadDefaultSettings()
        {
            return ReadSettings();
        }

        public DataModels.Settings ReadSettings()
        {
            return new DataModels.Settings();
        }
    }
}
