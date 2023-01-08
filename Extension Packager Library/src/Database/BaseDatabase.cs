using Microsoft.Data.Sqlite;
using System;

namespace Extension_Packager_Library.src.Database
{
    public class BaseDatabase
    {
        internal string DatabaseFile { get; set; }

        internal string CreateConnectionString()
        {
            return new SqliteConnectionStringBuilder()
            {
                Mode = SqliteOpenMode.ReadWriteCreate,
                DataSource = DatabaseFile,
                ForeignKeys = true
            }.ToString();
        }

        internal void ExecuteNonQueryCommand(Func<SqliteConnection, SqliteCommand> createCommand)
        {
            ExecuteCommand(async (connection) =>
            {
                SqliteCommand command = createCommand(connection);
                await command.ExecuteNonQueryAsync();
            });
        }

        internal void ExecuteReaderCommand(Func<SqliteConnection, SqliteCommand> createCommand, Action<SqliteDataReader> readData)
        {
            ExecuteCommand(async (connection) =>
            {
                SqliteCommand command = createCommand(connection);
                using SqliteDataReader reader = await command.ExecuteReaderAsync();
                readData(reader);
            });
        }

        internal void ExecuteCommand(Action<SqliteConnection> sqlFunction)
        {
            string connectionString = CreateConnectionString();
            using SqliteConnection connection = new(connectionString);
            SQLitePCL.Batteries.Init();
            connection.Open();

            sqlFunction(connection);

            connection.Close();
        }
    }
}
