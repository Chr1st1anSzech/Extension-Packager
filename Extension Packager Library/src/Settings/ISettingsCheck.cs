﻿// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Extension_Packager_Library.src.Settings
{
    public interface ISettingsCheck
    {
        public bool AreSettingsValide(DataModels.Settings settings);
    }
}
