// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Extension_Packager_Library.src.DataModels
{
    public class Extension
    {
        /// <summary>
        /// Full name of the extension.
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Shortened name of the extension. Used for the directory path.
        /// </summary>
        public string ShortName { get; set; }


        /// <summary>
        /// ID of the extension.
        /// </summary>
        public string Id { get; set; }


        /// <summary>
        /// Version of the extension.
        /// </summary>
        public string Version { get; set; }


        /// <summary>
        /// The date when the extension was created or updated.
        /// </summary>
        public DateTime UpdateDate { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string DeployementDir { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string BackupDir { get; set; }

    }
}
