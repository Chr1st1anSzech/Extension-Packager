// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace Extension_Packager_Library.src.DataModels
{
    public class Manifest
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly static string _updateUrlAttribute = "update_url";

        public string RawContent { get; set; }
        public string File { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }

        public Manifest(string rawContent, string file)
        {
            RawContent = rawContent;
            File = file;
        }

        /// <summary>
        /// Sets a new value for the update URL. The attribute is created if it does not already exist.
        /// </summary>
        /// <param name="updateUrl">The new value for the attribute.</param>
        public static string ChangeUpdateUrl(string manifest, string updateUrl)
        {
            try
            {
                JObject json = JObject.Parse(manifest);
                if (json[_updateUrlAttribute] == null)
                {
                    json.Add(_updateUrlAttribute);
                }
                json[_updateUrlAttribute] = updateUrl;
                return json.ToString();
            }
            catch (JsonReaderException jsonException)
            {
                _log.Error($"The manifest is no valide JSON.", jsonException);
                throw;
            }
            catch (Exception jsonException)
            {
                _log.Error($"The attribute \"{_updateUrlAttribute}\" could not be changed.", jsonException);
                throw;
            }
        }
    }
}
