// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager_Library.src.DataModels;
using Extension_Packager_Library.src.Helper;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Reflection;

namespace Extension_Packager_Library.src.Extension
{
    internal interface IManifestReader
    {
        internal Manifest ReadManifest();
    }

    internal class ManifestReader : IManifestReader
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _manifestFile;
        private Manifest _manifest;


        internal ManifestReader(string unpackedFolderPath)
        {
            _manifestFile = GetManifestPath(unpackedFolderPath);
        }


        private string GetManifestPath(string unpackedFolderPath)
        {
            return Path.Combine(unpackedFolderPath, "manifest.json");
        }
        
        
        /// <summary>
        /// Reads a CRX extension and inserts its values into the passed object. 
        /// The CRX file is converted to a normal ZIP file. 
        /// Then it is unpacked in the temporary directory.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public Manifest ReadManifest()
        {
            _log.Info($"Get manifest information from file.");
            _log.Debug($"Manifest file path is \"{_manifestFile}\"");

            if (!CheckIfManifestExists())
            {
                _log.Error($"The file \"{_manifestFile}\" was not found.");
                return null;
            }

            string manifest = ReadManifestText();
            if (manifest == null) return null;

            _manifest = new(manifest, _manifestFile);
            ExtractManifestInfos();
            return _manifest;
        }

        private bool CheckIfManifestExists()
        {
            return File.Exists(_manifestFile);
        }

        private string ReadManifestText()
        {
            _log.Debug("Read manifest text.");
            try
            {
                return File.ReadAllText(_manifestFile);
            }
            catch (Exception ex)
            {
                _log.Error($"The file \"{_manifestFile}\" could not be read.", ex);
                throw;
            }
        }

        private void ExtractManifestInfos()
        {
            _log.Debug("Parse manifest.");
            try
            {
                JObject json = JObject.Parse(_manifest.RawContent);
                _manifest.Name = JsonHelper.GetString(json, "name");
                _manifest.Version = JsonHelper.GetString(json, "version");
            }
            catch(Exception ex)
            {
                _log.Error("The manifest is not a valid JSON and could not be parsed.", ex);
                throw;
            }
        }
    }
}
