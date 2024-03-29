﻿using System.Collections.Generic;

namespace Extension_Packager_Library.src.Database
{
    internal interface IExtensionStorage
    {
        #region Table Last Modified


        /// <summary>
        /// Inserts or updates an extension in the table of the last created/updated ones.
        /// </summary>
        /// <param name="extension">The extension to be inserted or updated.</param>
        internal void SetLastModified(DataModels.Extension extension);


        /// <summary>
        /// Queries all the most recently created/updated extensions.
        /// </summary>
        /// <returns>All the most recently created/updated extensions.</returns>
        internal List<DataModels.Extension> GetAllLastModified();


        /// <summary>
        /// Queries the extension with the passed ID.
        /// </summary>
        /// <param name="id">The ID of the extension.</param>
        /// <returns>The extension with the specified ID or NULL if no extension was found.</returns>
        internal DataModels.Extension GetLastModified(string id);


        /// <summary>
        /// Deletes the extension from the table of last created/updated extensions.
        /// </summary>
        /// <param name="id">The ID of the extension to be deleted.</param>
        internal void DeleteLastModified(string id);


        #endregion


        #region Table All Extensions


        /// <summary>
        /// Inserts or updates an extension in the table of all extensions.
        /// </summary>
        /// <param name="extension">The extension to be inserted or updated.</param>
        internal void Set(DataModels.Extension extension);


        /// <summary>
        /// Queries all extensions from the storage.
        /// </summary>
        /// <returns>All extensions from the storage.</returns>
        internal List<DataModels.Extension> GetAll();


        /// <summary>
        /// Queries an extension with the passed ID.
        /// </summary>
        /// <param name="id">The ID of the extension that is being requested.</param>
        /// <returns>The extension with the passed ID.</returns>
        internal DataModels.Extension GetById(string id);



        /// <summary>
        /// Queries an extension with the passed shortname.
        /// </summary>
        /// <param name="shortname">The shortname of the extensions that is being requested.</param>
        /// <returns>The extensions with the passed shortname.</returns>
        internal int GetCountByShortname(string shortname);



        /// <summary>
        /// Queries an extension with the passed id.
        /// </summary>
        /// <param name="id">The id of the extensions that is being requested.</param>
        /// <returns>The extensions with the passed id.</returns>
        internal int GetCountById(string id);

        /// <summary>
        /// Deletes the extension with the passed ID from the storage.
        /// </summary>
        /// <param name="id">The ID of the extension to be deleted.</param>
        internal void Delete(string id);


        #endregion
    }
}
