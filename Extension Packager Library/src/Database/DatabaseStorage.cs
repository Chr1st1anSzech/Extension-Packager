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
        private readonly string CRX_COLUMN = "crx_path";
        private readonly string PRIVATE_KEY_COLUMN = "private_key_path";

        #endregion


        #region Static Fields

        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion




        public DatabaseStorage()
        {
            InitializeDatabase();
        }


        public void AddToLastUsedExtension(DataModels.Extension extension)
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



        public List<DataModels.Extension> ReadLastUsedExtensions()
        {
            List<DataModels.Extension> extensions = new();

            ExecuteReaderCommand((connection) =>
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @$"SELECT 
                    {EXTENSION_TABLE}.{ID_COLUMN},
                    {NAME_COLUMN},
                    {SHORTNAME_COLUMN},
                    {VERSION_COLUMN},
                    {CRX_COLUMN},
                    {PRIVATE_KEY_COLUMN}
                FROM {EXTENSION_TABLE}
                JOIN {LAST_USED_TABLE}
                ON {EXTENSION_TABLE}.{ID_COLUMN} = {LAST_USED_TABLE}.{ID_REF_COLUMN}";
                return command;
            },
            (dataReader) =>
            {
                while (dataReader.Read())
                {
                    DataModels.Extension extension = CreateExtensionFromRow(dataReader);
                    if (extension != null)
                    {
                        extensions.Add(extension);
                    }
                }
            });

            return extensions;
        }



        public void SaveExtension(DataModels.Extension extension)
        {
            ExecuteNonQueryCommand((connection) =>
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @$"INSERT INTO {EXTENSION_TABLE} (
                        {ID_COLUMN},
                        {NAME_COLUMN},
                        {SHORTNAME_COLUMN},
                        {VERSION_COLUMN},
                        {CRX_COLUMN},
                        {PRIVATE_KEY_COLUMN}
                )
                VALUES (
                    ${ID_COLUMN},
                    ${NAME_COLUMN},
                    ${SHORTNAME_COLUMN},
                    ${VERSION_COLUMN},
                    ${CRX_COLUMN},
                    ${PRIVATE_KEY_COLUMN}
                )";
                command.Parameters.AddWithValue($"${ID_COLUMN}", extension.Id);
                command.Parameters.AddWithValue($"${NAME_COLUMN}", extension.Name);
                command.Parameters.AddWithValue($"${SHORTNAME_COLUMN}", extension.ShortName);
                command.Parameters.AddWithValue($"${VERSION_COLUMN}", extension.Version);
                command.Parameters.AddWithValue($"${CRX_COLUMN}", extension.PackedCrxFile);
                command.Parameters.AddWithValue($"${PRIVATE_KEY_COLUMN}", extension.PrivateKeyFile);
                return command;
            });
        }



        public List<DataModels.Extension> ReadExtensions()
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
                    DataModels.Extension extension = CreateExtensionFromRow(dataReader);
                    if (extension != null)
                    {
                        extensions.Add(extension);
                    }
                }
            });

            return extensions;
        }


        public DataModels.Extension ReadExtension(string id)
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
                    WHERE {ID_COLUMN} = '{id}'";
                return command;
            },
            (dataReader) =>
            {
                if (dataReader.Read())
                {
                    extension = CreateExtensionFromRow(dataReader);
                }
            });

            return extension;
        }



        private DataModels.Extension CreateExtensionFromRow(SqliteDataReader dataReader)
        {
            try
            {
                DataModels.Extension ext = new()
                {
                    Id = dataReader.GetString(ID_COLUMN),
                    Name = dataReader.GetString(NAME_COLUMN),
                    ShortName = dataReader.GetString(SHORTNAME_COLUMN),
                    Version = dataReader.GetString(VERSION_COLUMN),
                    PackedCrxFile = dataReader.GetString(CRX_COLUMN),
                    PrivateKeyFile = dataReader.GetString(PRIVATE_KEY_COLUMN)
                };

                return ext;
            }
            catch (Exception exception)
            {
                _log.Warn("The record of an extension is invalid and is skipped.", exception);
            }

            return null;
        }



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
                    {CRX_COLUMN} TEXT,
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



        private (bool, string) TryGetDatabaseFile()
        {
            bool result = ResourceFile.TryGetFullPath(DATABASE_FILE_NAME, out string databaseFile);
            return (result, databaseFile);
        }
    }
}
