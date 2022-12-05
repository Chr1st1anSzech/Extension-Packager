// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using log4net;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace Extension_Packager_Library.src.Settings
{
    public class SettingsFileWriter : ISettingsWriter
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void WriteSettings(DataModels.Settings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException($"{nameof(WriteSettings)}: The value of the parameter ${nameof(settings)} is zero.");
            }

            SettingsFile.TryGetValidePath(out string path);
            File.WriteAllText(path, JsonConvert.SerializeObject(settings));
        }
    }
}
