using System;
using System.Data.SqlClient;
using static GeneralServices.Enums;

namespace GeneralServices.Helpers
{
    public static class DBHelper
    {
        /// <summary>
        /// Executes a SQL command that returns only affected rows number (ExecuteNonQuery)
        /// </summary>
        /// <param name="connection">An open SQL connection</param>
        /// <param name="script">The MSSQL script to be executed</param>
        /// <returns>True if affected row number is zero or greater</returns>
        public static bool executeSqlScript(SqlConnection connection, string script)
        {
            if (connection == null)
            {
                throw new Exception(string.Format("{0} : SQL Connection must not be null.", Reflection.GetCurrentMethodName()));
            }
            else if (connection.State != System.Data.ConnectionState.Open)
            {
                throw new Exception(string.Format("{0} : SQL Connection state is not ready. [State={1}]", Reflection.GetCurrentMethodName(), connection.State.ToString()));
            }
            if (string.IsNullOrEmpty(script))
            {
                throw new Exception(string.Format("{0} : SQL creation script cannot be empty.", Reflection.GetCurrentMethodName()));
            }

            int rows = Consts.SQL_INVALID_ROW_COUNT;
            bool result = true;

            string cmdString = script;

            try
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = cmdString;

                    rows = command.ExecuteNonQuery();
                }
            }
            catch (Exception sqlCommandEx)
            {
                result = false;
                throw sqlCommandEx;
            }

            return result;
        }
        /// <summary>
        /// Runs a transaction command based on the parameters (BEGIN, COMMIT and ROLLBACK)
        /// </summary>
        /// <param name="connection">Active SqlCommation that the transaction command will be applied upn</param>
        /// <param name="TransactionName">Name of the transaction</param>
        /// <param name="Command">Predefined SQL Server transaction command lookup</param>
        /// <returns>True if begin transaction succeeded</returns>
        public static bool Transaction(SqlConnection connection, string TransactionName, SQL_TransactionCommands Command)
        {
            bool actionResult = false;
            int rows = Consts.SQL_INVALID_ROW_COUNT;

            if (string.IsNullOrEmpty(TransactionName))
            {
                throw new Exception("{0} : Transaction name cannot be empty");
            }

            using (SqlCommand command = new SqlCommand())
            {
                string cmdString = string.Format("{0} TRANSACTION {1}", Consts.SQL_TRAN_COMMANDS[(int)Command], TransactionName);
                command.Connection = connection;
                command.CommandText = cmdString;

                command.ExecuteNonQuery();

                // Check transaction state
                cmdString = string.Format("SELECT COUNT(*) FROM sys.dm_tran_active_transactions WHERE name = '{0}'", TransactionName);
                command.CommandText = cmdString;

                rows = (int)command.ExecuteScalar();
                if (Command == SQL_TransactionCommands.Begin)
                {
                    if (rows > Consts.SQL_NO_ROWS_AFFECTED)
                    {
                        actionResult = true;
                    }
                }
                else
                {
                    actionResult = true;
                }

            }

            return actionResult;
        }

    }
}
