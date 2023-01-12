// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Extension_Packager_Library.src.Extension
{
    public class ExtensionManager
    {
        private static ExtensionManager _instance;
        public static ExtensionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new ArgumentNullException($"The {nameof(ExtensionManager)} was not initialized.");
                }
                else
                {
                    return _instance;
                }
            }
        }

        //public DataModels.Extension CurrentExtension { get; set; }

        private ExtensionManager() { }

        public static void Initialize()
        {
            _instance = new()
            {

            };
        }
    }
}
