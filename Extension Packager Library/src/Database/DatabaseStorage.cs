﻿using Extension_Packager_Library.src.Helper;
using log4net;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Extension_Packager_Library.src.Database
{
    internal class DatabaseStorage : BaseDatabase, IExtensionStorage
    {
        #region Constants
        private readonly int MAXIMUM_LAST_USED_ROWS = 10;

        private readonly string DATABASE_FILE_NAME = "extensions.sqlite";

        private readonly string EXTENSION_TABLE = "extensions";
        private readonly string LAST_USED_TABLE = "last_used";

        private readonly string DATE_COLUMN = "add_datetime";
        private readonly string NAME_COLUMN = "name";
        private readonly string SHORTNAME_COLUMN = "shortname";
        private readonly string VERSION_COLUMN = "version";
        private readonly string ID_COLUMN = "id";
        private readonly string ID_REF_COLUMN = "ext_id";
        private readonly string DEPLOYEMENT_DIR_COLUMN = "deployement_dir";
        private readonly string BACKUP_DIR_COLUMN = "backup_dir";
        //private readonly string PRIVATE_KEY_COLUMN = "private_key_path";

        #endregion


        #region Static Fields

        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion


        public DatabaseStorage()
        {
            InitializeDatabase();
        }


        #region Table Last Modified


        /// <summary>
        /// Inserts or updates an extension in the table of the last created/updated ones.
        /// </summary>
        /// <param name="extension">The extension to be inserted or updated.</param>
        public void SetLastModified(DataModels.Extension extension)
        {
            if (GetLastModified(extension.Id) == null)
            {
                InsertLastModified(extension);
            }
            else
            {
                UpdateLastModified(extension);
            }
        }


        /// <summary>
        /// Inserts an extension in the table of the last created/updated ones.
        /// </summary>
        /// <param name="extension">The extension to be inserted.</param>
        private void InsertLastModified(DataModels.Extension extension)
        {
            ExecuteNonQueryCommand((connection) =>
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @$"INSERT INTO {LAST_USED_TABLE} (
                        {ID_REF_COLUMN}
                )
                VALUES (
                    ${ID_REF_COLUMN}
                )";
                command.Parameters.AddWithValue($"${ID_REF_COLUMN}", extension.Id);
                return command;
            });
        }


        /// <summary>
        /// Updates an extension in the table of the last created/updated ones.
        /// </summary>
        /// <param name="extension">The extension to be updated.</param>
        private void UpdateLastModified(DataModels.Extension extension)
        {
            ExecuteNonQueryCommand((connection) =>
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @$"UPDATE {LAST_USED_TABLE}
                SET
                    {DATE_COLUMN} = ${DATE_COLUMN}
                WHERE {ID_REF_COLUMN} = ${ID_REF_COLUMN}";
                command.Parameters.AddWithValue($"${DATE_COLUMN}", DateTime.Now);
                command.Parameters.AddWithValue($"${ID_REF_COLUMN}", extension.Id);
                return command;
            });
        }


        /// <summary>
        /// Queries all the most recently created/updated extensions.
        /// </summary>
        /// <returns>All the most recently created/updated extensions.</returns>
        public List<DataModels.Extension> GetAllLastModified()
        {
            List<DataModels.Extension> extensions = new();

            ExecuteReaderCommand((connection) =>
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @$"SELECT 
                    {EXTENSION_TABLE}.{ID_COLUMN},
                    {LAST_USED_TABLE}.{DATE_COLUMN},
                    {NAME_COLUMN},
                    {SHORTNAME_COLUMN},
                    {VERSION_COLUMN},
                    {DEPLOYEMENT_DIR_COLUMN},
                    {BACKUP_DIR_COLUMN}
                FROM {EXTENSION_TABLE}
                JOIN {LAST_USED_TABLE}
                ON {EXTENSION_TABLE}.{ID_COLUMN} = {LAST_USED_TABLE}.{ID_REF_COLUMN}
                ORDER BY {LAST_USED_TABLE}.{DATE_COLUMN} DESC";
                return command;
            },
            (dataReader) =>
            {
                while (dataReader.Read())
                {
                    DataModels.Extension extension = ParseRow(dataReader, (dataReader, ext) =>
                    {
                        ext.UpdateDate = dataReader.GetDateTime(DATE_COLUMN);
                    });

                    if (extension != null)
                    {
                        extensions.Add(extension);
                    }
                }
            });

            return extensions;
        }


        /// <summary>
        /// Queries the extension with the passed ID.
        /// </summary>
        /// <param name="id">The ID of the extension.</param>
        /// <returns>The extension with the specified ID or NULL if no extension was found.</returns>
        public DataModels.Extension GetLastModified(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _log.Warn($"\"{nameof(id)}\" must not be NULL or a space character.");
                return null;
            }

            DataModels.Extension extension = null;

            ExecuteReaderCommand((connection) =>
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @$"SELECT 
                    {EXTENSION_TABLE}.{ID_COLUMN},
                    {LAST_USED_TABLE}.{DATE_COLUMN},
                    {NAME_COLUMN},
                    {SHORTNAME_COLUMN},
                    {VERSION_COLUMN},
                    {DEPLOYEMENT_DIR_COLUMN},
                    {BACKUP_DIR_COLUMN}
                FROM {EXTENSION_TABLE}
                JOIN {LAST_USED_TABLE}
                ON {EXTENSION_TABLE}.{ID_COLUMN} = {LAST_USED_TABLE}.{ID_REF_COLUMN}
                WHERE {ID_REF_COLUMN} = ${ID_COLUMN}";
                command.Parameters.AddWithValue($"${ID_COLUMN}", id);
                return command;
            },
            (dataReader) =>
            {
                if (dataReader.Read())
                {
                    extension = ParseRow(dataReader);
                }
            });

            return extension;
        }


        /// <summary>
        /// Deletes the extension from the table of last created/updated extensions.
        /// </summary>
        /// <param name="id">The ID of the extension to be deleted.</param>
        public void DeleteLastModified(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _log.Warn($"\"{nameof(id)}\" must not be NULL or a space character.");
                return;
            }

            ExecuteNonQueryCommand((connection) =>
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @$"DELETE FROM {LAST_USED_TABLE}
                    WHERE {ID_REF_COLUMN} = ${ID_REF_COLUMN}";
                command.Parameters.AddWithValue($"${ID_REF_COLUMN}", id);
                return command;
            });
        }


        #endregion


        #region Table All Extensions

        /// <summary>
        /// Inserts or updates an extension in the table of all extensions.
        /// </summary>
        /// <param name="extension">The extension to be inserted or updated.</param>
        public void Set(DataModels.Extension extension)
        {
            if (GetById(extension.Id) == null)
            {
                Insert(extension);
            }
            else
            {
                Update(extension);
            }
        }


        /// <summary>
        /// Inserts an extension in the table of all extensions.
        /// </summary>
        /// <param name="extension">The extension to be inserted.</param>
        private void Insert(DataModels.Extension extension)
        {
            ExecuteNonQueryCommand((connection) =>
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @$"INSERT INTO {EXTENSION_TABLE} (
                        {ID_COLUMN},
                        {NAME_COLUMN},
                        {SHORTNAME_COLUMN},
                        {VERSION_COLUMN},
                        {DEPLOYEMENT_DIR_COLUMN},
                        {BACKUP_DIR_COLUMN}
                )
                VALUES (
                    ${ID_COLUMN},
                    ${NAME_COLUMN},
                    ${SHORTNAME_COLUMN},
                    ${VERSION_COLUMN},
                    ${DEPLOYEMENT_DIR_COLUMN},
                    ${BACKUP_DIR_COLUMN}
                )";
                command.Parameters.AddWithValue($"${ID_COLUMN}", extension.Id);
                command.Parameters.AddWithValue($"${NAME_COLUMN}", extension.Name);
                command.Parameters.AddWithValue($"${SHORTNAME_COLUMN}", extension.ShortName);
                command.Parameters.AddWithValue($"${VERSION_COLUMN}", extension.Version);
                command.Parameters.AddWithValue($"${DEPLOYEMENT_DIR_COLUMN}", extension.DeployementDir);
                command.Parameters.AddWithValue($"${BACKUP_DIR_COLUMN}", extension.BackupDir);
                return command;
            });
        }


        /// <summary>
        /// Updates an extension in the table of all extensions.
        /// </summary>
        /// <param name="extension">The extension to be updated.</param>
        private void Update(DataModels.Extension extension)
        {
            ExecuteNonQueryCommand((connection) =>
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @$"UPDATE {EXTENSION_TABLE}
                SET
                    {VERSION_COLUMN} = ${VERSION_COLUMN}
                WHERE {ID_COLUMN} = ${ID_COLUMN}";
                command.Parameters.AddWithValue($"${VERSION_COLUMN}", extension.Version);
                command.Parameters.AddWithValue($"${ID_COLUMN}", extension.Id);
                return command;
            });
        }


        /// <summary>
        /// Queries all extensions from the storage.
        /// </summary>
        /// <returns>All extensions from the storage.</returns>
        public List<DataModels.Extension> GetAll()
        {
            List<DataModels.Extension> extensions = new();

            ExecuteReaderCommand((connection) =>
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @$"SELECT * FROM {EXTENSION_TABLE}";
                return command;
            },
            (dataReader) =>
            {
                while (dataReader.Read())
                {
                    DataModels.Extension extension = ParseRow(dataReader);
                    if (extension != null)
                    {
                        extensions.Add(extension);
                    }
                }
            });

            return extensions;
        }


        /// <summary>
        /// Queries an extension with the passed ID.
        /// </summary>
        /// <param name="id">The ID of the extension that is being requested.</param>
        /// <returns>The extension with the passed ID.</returns>
        public DataModels.Extension GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _log.Warn($"\"{nameof(id)}\" must not be NULL or a space character.");
                return null;
            }

            DataModels.Extension extension = null;

            ExecuteReaderCommand((connection) =>
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @$"SELECT * FROM {EXTENSION_TABLE}
                    WHERE {ID_COLUMN} = ${ID_COLUMN}";
                command.Parameters.AddWithValue($"${ID_COLUMN}", id);
                return command;
            },
            (dataReader) =>
            {
                if (dataReader.Read())
                {
                    extension = ParseRow(dataReader);
                }
            });

            return extension;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="shortname"></param>
        /// <returns></returns>
        public int GetCountByShortname(string shortname)
        {
            int count = 0;
            if (string.IsNullOrWhiteSpace(shortname))
            {
                _log.Warn($"\"{nameof(shortname)}\" must not be NULL or a space character.");
                return count;
            }
            ExecuteReaderCommand((connection) =>
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @$"SELECT COUNT({ID_COLUMN}) AS COUNT FROM {EXTENSION_TABLE}
                    WHERE {SHORTNAME_COLUMN} = ${SHORTNAME_COLUMN}";
                command.Parameters.AddWithValue($"${SHORTNAME_COLUMN}", shortname);
                return command;
            },
            (dataReader) =>
            {
                if (dataReader.Read())
                {
                    count = dataReader.GetInt32("COUNT");
                }
            });

            return count;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetCountById(string id)
        {
            int count = 0;
            if (string.IsNullOrWhiteSpace(id))
            {
                _log.Warn($"\"{nameof(id)}\" must not be NULL or a space character.");
                return count;
            }
            ExecuteReaderCommand((connection) =>
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @$"SELECT COUNT({ID_COLUMN}) AS COUNT FROM {EXTENSION_TABLE}
                    WHERE {ID_COLUMN} = ${ID_COLUMN}";
                command.Parameters.AddWithValue($"${ID_COLUMN}", id);
                return command;
            },
            (dataReader) =>
            {
                if (dataReader.Read())
                {
                    count = dataReader.GetInt32("COUNT");
                }
            });

            return count;
        }


        /// <summary>
        /// Deletes the extension with the passed ID from the storage.
        /// </summary>
        /// <param name="id">The ID of the extension to be deleted.</param>
        public void Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _log.Warn($"\"{nameof(id)}\" must not be NULL or a space character.");
                return;
            }

            ExecuteNonQueryCommand((connection) =>
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @$"DELETE FROM {EXTENSION_TABLE}
                    WHERE {ID_COLUMN} = ${ID_COLUMN}";
                command.Parameters.AddWithValue($"${ID_COLUMN}", id);
                return command;
            });
        }


        /// <summary>
        /// Reads a data set and creates an extension object from it.
        /// </summary>
        /// <param name="dataReader">The DataReader object of an SQL query.</param>
        /// <param name="parseAdditionalData">The function to read out further properties.</param>
        /// <returns>The extension object or NULL if no record was found or the connection is already closed./returns>
        private DataModels.Extension ParseRow(SqliteDataReader dataReader, Action<SqliteDataReader, DataModels.Extension> parseAdditionalData = null)
        {
            try
            {
                if (!dataReader.HasRows || dataReader.IsClosed) return null;

                DataModels.Extension ext = new()
                {
                    Id = dataReader.GetString(ID_COLUMN),
                    Name = dataReader.GetString(NAME_COLUMN),
                    ShortName = dataReader.GetString(SHORTNAME_COLUMN),
                    Version = dataReader.GetString(VERSION_COLUMN),
                    DeployementDir = dataReader.GetString(DEPLOYEMENT_DIR_COLUMN),
                    BackupDir = dataReader.GetString(BACKUP_DIR_COLUMN)
                };

                if (parseAdditionalData != null)
                {
                    parseAdditionalData(dataReader, ext);
                }

                return ext;
            }
            catch (Exception exception)
            {
                _log.Warn("The record of an extension is invalid and is skipped.", exception);
            }

            return null;
        }

        #endregion


        #region Database Initialization


        /// <summary>
        /// Checks the existence of the database file and otherwise creates the database and tables.
        /// </summary>
        private void InitializeDatabase()
        {
            (bool, string) result = TryGetDatabaseFile();
            DatabaseFile = result.Item2;
            if (!result.Item1)
            {
                CreateExtensionTable();
                CreateLastUsedTable();
            }
        }


        /// <summary>
        /// Creates the table in which all extensions are inserted.
        /// </summary>
        private void CreateExtensionTable()
        {
            ExecuteNonQueryCommand((connection) =>
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText =
                @$"CREATE TABLE {EXTENSION_TABLE}(
                    {ID_COLUMN} TEXT NOT NULL PRIMARY KEY,
                    {NAME_COLUMN} TEXT NOT NULL,
                    {SHORTNAME_COLUMN} TEXT NOT NULL,
                    {VERSION_COLUMN} TEXT NOT NULL,
                    {DEPLOYEMENT_DIR_COLUMN} TEXT NOT NULL,
                    {BACKUP_DIR_COLUMN} TEXT NOT NULL
                )";
                return command;
            });
        }


        /// <summary>
        /// Creates the table in which all recently updated extensions are inserted.
        /// </summary>
        private void CreateLastUsedTable()
        {
            ExecuteNonQueryCommand((connection) =>
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText =
                @$"CREATE TABLE {LAST_USED_TABLE}(
                    {ID_COLUMN} INTEGER PRIMARY KEY AUTOINCREMENT,
                    {ID_REF_COLUMN} TEXT NOT NULL,
                    {DATE_COLUMN} DATETIME DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY({ID_REF_COLUMN}) REFERENCES {EXTENSION_TABLE}({ID_COLUMN})
                )";
                return command;
            });

            ExecuteNonQueryCommand((connection) =>
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText =
                @$"CREATE TRIGGER {LAST_USED_TABLE}_trigger
                    AFTER INSERT
                    ON {LAST_USED_TABLE}
                    WHEN (SELECT COUNT(*) FROM {LAST_USED_TABLE}) > {MAXIMUM_LAST_USED_ROWS}
                BEGIN
                    DELETE FROM {LAST_USED_TABLE}
                        WHERE {ID_COLUMN} IN
                        ( 
                            SELECT {ID_COLUMN} FROM {LAST_USED_TABLE}
                            ORDER BY {DATE_COLUMN} ASC LIMIT 1
                        );
                END
                ";
                return command;
            });
        }

        #endregion


        /// <summary>
        /// Links the file name of the database file with the application path.
        /// Checks whether the file exists and returns the result and the complete file path.
        /// </summary>
        /// <returns>The result whether the database file exists and the full file path.</returns>
        private (bool, string) TryGetDatabaseFile()
        {
            bool result = ResourceFile.TryGetFullPath(DATABASE_FILE_NAME, out string databaseFile);
            return (result, databaseFile);
        }
    }
}
