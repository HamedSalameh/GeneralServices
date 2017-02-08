using GeneralServices.Helpers;
using GeneralServices.Models;
using System;
using System.Data.SqlClient;

namespace GeneralServices.Services
{
    internal static class HistoryServiceDBHelper
    {
        internal static void createHistoryLogTable(string ConnectionString)
        {
            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new Exception(string.Format("{0} : Connection string value cannot be empty.", Reflection.GetCurrentMethodName()));
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    string commandString = string.Format(
                        "IF NOT EXISTS(SELECT * FROM sys.tables WHERE object_id = object_id('{0}')) " +
                        "   BEGIN " +
                        "       CREATE TABLE {0} " +
                        "           (" +
                        "           HistoryLogID INT PRIMARY KEY IDENTITY," +
                        "           EntityTypeLookup INT NOT NULL," +
                        "           EntityID INT NOT NULL," +
                        "           EntityOwnerID INT," +
                        "           Date DATETIME NOT NULL," +
                        "           CRUDType TINYINT NOT NULL," +
                        "           HashID INT" +
                        "           ) " +
                        "END "
                        , Consts.SQL_TABLES_HISTORY_HISTORYLOG);

                    DBHelper.executeSqlScript(connection, commandString);

                    connection.Close();
                }
            }
            catch (System.Exception Ex)
            {
                throw Ex;
            }
        }

        internal static bool validateHistoryLogTable(string ConnectionString)
        {
            bool result = false;

            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new Exception(string.Format("{0} : Connection string value cannot be empty.", Reflection.GetCurrentMethodName()));
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    string commandString = string.Format("SELECT COUNT(*) FROM sys.tables WHERE object_id = object_id('{0}') "
                        , Consts.SQL_TABLES_HISTORY_HISTORYLOG);

                    try
                    {
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = connection;
                            command.CommandText = commandString;

                            int rows = (int)command.ExecuteScalar();
                            result = rows > Consts.SQL_INVALID_ROW_COUNT;
                        }
                    }
                    catch (Exception sqlCommandEx)
                    {
                        result = false;
                        throw sqlCommandEx;
                    }


                    connection.Close();
                }
            }
            catch (System.Exception Ex)
            {
                throw Ex;
            }

            return result;
        }

        public static bool AddEntityHistoryEntry(HistoryLog EntityEntry, string ConnectionString)
        {
            bool result = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = string.Format("INSERT INTO {0} (EntityTypeLookup, EntityID, EntityOwnerID, Date, CRUDType, HashID) "+
                            "VALUES ({1},{2},{3},'{4}','{5}',{6})", Consts.SQL_TABLES_HISTORY_HISTORYLOG,
                            EntityEntry.EntityTypeID,
                            EntityEntry.EntityID,
                            EntityEntry.EntityOwnerID,
                            EntityEntry.Date,
                            (int)EntityEntry.CRUDType,
                            EntityEntry.HashID
                            );

                        int rows = command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }
            catch (System.Exception Ex)
            {
                throw Ex;
            }


            return result;
        }
    }
}
