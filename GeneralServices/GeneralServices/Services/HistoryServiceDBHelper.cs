using GeneralServices.Helpers;
using GeneralServices.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GeneralServices.Services
{
    internal static class HistoryServiceDBHelper
    {
        private static DataTable prepareDataTable(List<EntityPropertyChange> Changes)
        {
            DataTable dtChanges = new DataTable();
            DataRow dtRow;

            dtChanges.Columns.Add("HistoryLogID", typeof(int));
            dtChanges.Columns.Add("EntityPropertyID", typeof(int));
            dtChanges.Columns.Add("CurrentValueAsText", typeof(string));
            dtChanges.Columns.Add("OriginalValueAsText", typeof(string));
            dtChanges.Columns.Add("Date", typeof(DateTime));
            dtChanges.Columns.Add("HashID", typeof(int));

            if (Changes != null && Changes.Count > 0)
            {
                foreach (var change in Changes)
                {
                    dtRow = dtChanges.NewRow();
                    dtRow["HistoryLogID"] = change.HistoryLogID;
                    dtRow["EntityPropertyID"] = change.EntityPropertyID;
                    dtRow["CurrentValueAsText"] = change.CurrentValueAsText;
                    dtRow["OriginalValueAsText"] = change.OriginalValueAsText;
                    dtRow["Date"] = change.Date;
                    dtRow["HashID"] = change.HashID;

                    dtChanges.Rows.Add(dtRow);
                }
            }

            return dtChanges;
        }

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
                        "           EntityOwnerID INT DEFAULT 0," +
                        "           Date DATETIME NOT NULL," +
                        "           CRUDType TINYINT NOT NULL," +
                        "           ActionUserID INT DEFAULT 0," +
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

        internal static void createEntityPropertyChangesTable(string ConnectionString)
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
                        "           EntityPropertyChangeID INT PRIMARY KEY IDENTITY," +
                        "           HistoryLogID INT NOT NULL, " +
                        "           EntityPropertyID INT NOT NULL, " +
                        "           CurrentValueAsText NVARCHAR(200), " +
                        "           OriginalValueAsText NVARCHAR(200), " +
                        "           Date DATETIME NOT NULL, " +
                        "           HashID INT, " +

                        "           CONSTRAINT FK_HistoryLogID FOREIGN KEY (HistoryLogID) REFERENCES {1}(HistoryLogID), " +
                        "           ) " +
                        "END "
                        , Consts.SQL_TABLES_HISTORY_ENTITYPROPERTYCHANGES
                        , Consts.SQL_TABLES_HISTORY_HISTORYLOG
                        , Consts.SQL_TABLES_ENTITY_PROPERTY_LOOKUP_TABLE);

                    DBHelper.executeSqlScript(connection, commandString);

                    connection.Close();
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        internal static bool validateEntityPropertyChangesTable(string ConnectionString)
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
                        , Consts.SQL_TABLES_ENTITY_PROPERTY_LOOKUP_TABLE);

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

        internal static void createUDT_EntityPropertChangesTable(string ConnectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    string cmdString = string.Format(
                                        "IF NOT EXISTS(select * from sys.types where name = '{0}')" +
                                        "BEGIN " +
                                        "CREATE TYPE {0} AS TABLE(" +
                                        "   HistoryLogID INT NOT NULL, " +
                                        "   EntityPropertyID INT NOT NULL, " +
                                        "   CurrentValueAsText NVARCHAR(MAX), " +
                                        "   OriginalValueAsText NVARCHAR(MAX), " +
                                        "   Date DATETIME NOT NULL," +
                                        "   HashID INT NOT NULL" +
                                        ") " +
                                        "END "
                            , Consts.SQL_TYPES_UDT_HistoryServiceDBHelper_EntityProperties);

                    try
                    {
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = connection;
                            command.CommandText = cmdString;

                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception sqlCommandEx)
                    {
                        throw sqlCommandEx;
                    }

                    connection.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        internal static void createUSP_InsertEntityPropertyChanges(string connectionString)
        {
            string cmdString = string.Format(
                "IF EXISTS(select * from sys.procedures where object_id = object_id(('{0}'))) " +
                "BEGIN " +
                    "DROP PROCEDURE {0} " +
                "END "
                , Consts.SQL_PROCEDURES_USP_HistoryServiceDBHelper_InsertIntoEntityPropertyChangesTable);

            string cmdProcString = string.Format(

                "CREATE PROCEDURE {0} " +
                    "@dtEntityPropertyChanges {1} READONLY " +
                "AS " +
                "    BEGIN " +
                "       INSERT INTO {2} " +
                "       SELECT * FROM @dtEntityPropertyChanges " +
                "    END "
                , Consts.SQL_PROCEDURES_USP_HistoryServiceDBHelper_InsertIntoEntityPropertyChangesTable
                , Consts.SQL_TYPES_UDT_HistoryServiceDBHelper_EntityProperties
                , Consts.SQL_TABLES_HISTORY_ENTITYPROPERTYCHANGES);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    try
                    {
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = connection;

                            command.CommandText = cmdString;
                            command.ExecuteNonQuery();

                            command.CommandText = cmdProcString;
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception sqlCommandEx)
                    {
                        throw sqlCommandEx;
                    }

                    connection.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// Inserts a new entity history entry and returns the history log ID
        /// </summary>
        /// <param name="EntityEntry"></param>
        /// <param name="ConnectionString"></param>
        /// <returns>NEw history log ID</returns>
        internal static int AddEntityHistoryEntry(HistoryLog EntityEntry, string ConnectionString)
        {
            int HistoryLogID = Consts.INVALID_INDEX;

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = string.Format("INSERT INTO {0} (EntityTypeLookup, EntityID, EntityOwnerID, Date, CRUDType, HashID) "+
                            " OUTPUT INSERTED.HistoryLogID " +
                            " VALUES (@etype,@eid,@eownerid,@udate,@crud,@hash); "
                            , Consts.SQL_TABLES_HISTORY_HISTORYLOG
                            );

                        command.Parameters.AddWithValue("@etype", EntityEntry.EntityTypeID);
                        command.Parameters.AddWithValue("@eid", EntityEntry.EntityID);
                        command.Parameters.AddWithValue("@eownerid", EntityEntry.EntityOwnerID);

                        var updateDate = new SqlParameter("@udate", SqlDbType.DateTime2);
                        updateDate.Value = EntityEntry.Date;
                        command.Parameters.Add(updateDate);
                        
                        command.Parameters.AddWithValue("@crud",(int)EntityEntry.CRUDType);
                        command.Parameters.AddWithValue("@hash", EntityEntry.HashID);

                        int rows = (int)command.ExecuteScalar();

                        if (rows != Consts.SQL_INVALID_ROW_COUNT)
                        {
                            HistoryLogID = rows;
                        } 
                    }

                    connection.Close();
                }
            }
            catch (System.Exception Ex)
            {
                throw Ex;
            }


            return HistoryLogID;
        }

        internal static void AddEntityPropertyChangesHistoryLogs(List<EntityPropertyChange> Changes, string ConnectionString)
        {
            // try to save entity propery changes
            if (Changes != null && Changes.Count > 0)
            {
                // convert the list of changes to datatable
                DataTable dtChange = prepareDataTable(Changes);

                try
                {
                    using (SqlConnection connection = new SqlConnection(ConnectionString))
                    {
                        connection.Open();

                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = connection;
                            command.CommandType = CommandType.StoredProcedure;
                            command.CommandText = Consts.SQL_PROCEDURES_USP_HistoryServiceDBHelper_InsertIntoEntityPropertyChangesTable;

                            SqlParameter changesParam = command.Parameters.AddWithValue("@dtEntityPropertyChanges", dtChange);
                            changesParam.SqlDbType = SqlDbType.Structured;

                            command.ExecuteNonQuery();
                        }

                        connection.Close();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        internal static DataTable GetEntityHistory(int EntityID, string ConnectionString)
        {
            DataTable entityHistoryLog = new DataTable();

            if (EntityID != 0)
            {
                try
                {
                    string commandString = string.Format("SELECT * FROM {0} WHERE EntityID = {1}", Consts.SQL_TABLES_HISTORY_HISTORYLOG, EntityID);

                    using (SqlConnection connection = new SqlConnection(ConnectionString))
                    {
                        connection.Open();

                        using (SqlCommand command = new SqlCommand())
                        {

                            command.Connection = connection;
                            command.CommandText = commandString;

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                entityHistoryLog.Load(reader);
                            }

                        }

                        connection.Close();
                    }
                }
                catch (Exception sqlEx)
                {
                    throw sqlEx;
                }
            }

            return entityHistoryLog;
        }
    }
}
