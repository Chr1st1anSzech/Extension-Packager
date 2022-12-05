// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace Extension_Packager_Library.src.Helper
{
    public class JsonHelper
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public static string GetString(JObject json, string property)
        {
            if (json[property] == null)
            {
                _log.Error($"There is no JSON attribute with name \"{nameof(property)}\".");
                throw new ArgumentException(nameof(property));
            }
            return (string)json[property];
        }
    }
}
