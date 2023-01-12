using Extension_Packager_Library.src.DataModels;
using Extension_Packager_Library.src.Helper;
using log4net;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Extension_Packager_Library.src.Database
{
    public class DatabaseStorage : BaseDatabase, IExtensionStorage
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
        private readonly string PRIVATE_KEY_COLUMN = "private_key_path";

        #endregion


        #region Static Fields

        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion




        public DatabaseStorage()
        {
            InitializeDatabase();
        }

        #region Database Modification & Request

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
                    {PRIVATE_KEY_COLUMN}
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
                    {PRIVATE_KEY_COLUMN}
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

        public void Set(DataModels.Extension extension)
        {
            if( Get(extension.Id) == null)
            {
                Insert(extension);
            }
            else
            {
                Update(extension);
            }
        }

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
                        {PRIVATE_KEY_COLUMN}
                )
                VALUES (
                    ${ID_COLUMN},
                    ${NAME_COLUMN},
                    ${SHORTNAME_COLUMN},
                    ${VERSION_COLUMN},
                    ${PRIVATE_KEY_COLUMN}
                )";
                command.Parameters.AddWithValue($"${ID_COLUMN}", extension.Id);
                command.Parameters.AddWithValue($"${NAME_COLUMN}", extension.Name);
                command.Parameters.AddWithValue($"${SHORTNAME_COLUMN}", extension.ShortName);
                command.Parameters.AddWithValue($"${VERSION_COLUMN}", extension.Version);
                command.Parameters.AddWithValue($"${PRIVATE_KEY_COLUMN}", extension.PrivateKeyFile);
                return command;
            });
        }

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


        public DataModels.Extension Get(string id)
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


        private DataModels.Extension ParseRow(SqliteDataReader dataReader, Action<SqliteDataReader, DataModels.Extension> parseAdditionalData = null)
        {
            try
            {
                DataModels.Extension ext = new()
                {
                    Id = dataReader.GetString(ID_COLUMN),
                    Name = dataReader.GetString(NAME_COLUMN),
                    ShortName = dataReader.GetString(SHORTNAME_COLUMN),
                    Version = dataReader.GetString(VERSION_COLUMN),
                    PrivateKeyFile = dataReader.GetString(PRIVATE_KEY_COLUMN)
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



        private void CreateExtensionTable()
        {
            ExecuteNonQueryCommand((connection) =>
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText =
                @$"CREATE TABLE {EXTENSION_TABLE}(
                    {ID_COLUMN} TEXT NOT NULL PRIMARY KEY,
                    {NAME_COLUMN} TEXT,
                    {SHORTNAME_COLUMN} TEXT,
                    {VERSION_COLUMN} TEXT,
                    {PRIVATE_KEY_COLUMN} TEXT
                )";
                return command;
            });
        }



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


        private (bool, string) TryGetDatabaseFile()
        {
            bool result = ResourceFile.TryGetFullPath(DATABASE_FILE_NAME, out string databaseFile);
            return (result, databaseFile);
        }
    }
}
