// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using log4net;
using System;
using System.Reflection;

namespace Extension_Packager_Library.src.Settings
{
    public class SettingsRepository
    {
        public bool AreDefaultSettings { get; set; } = false;

        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static SettingsRepository _instance;
        public static SettingsRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new ArgumentNullException("The repository was not initialized.");
                }
                else
                {
                    return _instance;
                }
            }
        }
        
        private DataModels.Settings _settings;
        private ISettingsReader _settingsReader;
        private ISettingsWriter _settingsWriter;

        private SettingsRepository() { }

        public static void Initialize(ISettingsReader reader, ISettingsWriter writer)
        {
            _instance = new()
            {
                _settingsWriter = writer,
                _settingsReader = reader
            };
        }

        public void WriteSettings(DataModels.Settings settings)
        {
            _settings = settings;
            _settingsWriter.WriteSettings(settings);
        }

        public DataModels.Settings ReadSettings(bool readFromDisk = false)
        {
            if (_settings != null && !readFromDisk) return _settings;

            DataModels.Settings settings;
            try
            {
                settings = _settingsReader.ReadSettings();
                AreDefaultSettings = false;
            }
            catch (Exception e)
            {
                _log.Error("The configuration file could not be loaded. The default settings are used instead.", e);
                AreDefaultSettings = true;
                settings = _settingsReader.ReadDefaultSettings();
            }
            _settings = settings;
            return settings;
        }
    }
}
