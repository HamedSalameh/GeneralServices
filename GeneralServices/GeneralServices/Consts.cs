namespace GeneralServices
{
    public static class Consts
    {
        public static readonly int INVALID_INDEX = -1;

        public static readonly int SQL_INVALID_ROW_COUNT = -1;
        public static readonly int SQL_NO_ROWS_AFFECTED = 0;

        public static readonly string SQL_TRAN_COMMAND_BEGIN = "BEGIN ";
        public static readonly string SQL_TRAN_COMMAND_COMMIT = "COMMIT ";
        public static readonly string SQL_TRAN_COMMAND_ROLLBACK = "ROLLBACK ";
        public static readonly string[] SQL_TRAN_COMMANDS = { SQL_TRAN_COMMAND_BEGIN, SQL_TRAN_COMMAND_COMMIT, SQL_TRAN_COMMAND_ROLLBACK };

        public static readonly string SQL_TYPES_UDT_DomainMapperHelper_EntityProperties = "UDT_DomainMapperHelper_EntityProperties";
        public static readonly string SQL_TYPES_UDT_HistoryServiceDBHelper_EntityProperties = "UDT_HistoryServiceDBHelper_EntityPropertyChanges";

        public static readonly string SQL_PROCEDURES_USP_DomainMapperHelper_InsertIntoEntityPropertyLookupTable = "USP_DomainMapperHelper_InsertIntoEntityPropertyLookupTable";
        public static readonly string SQL_PROCEDURES_USP_HistoryServiceDBHelper_InsertIntoEntityPropertyChangesTable = "USP_HistoryServiceDBHelper_InsertIntoEntityPropertyChangesTable";

        public static readonly string SQL_TABLES_ENTITY_TYPE_LOOKUP_TABLE = "EntityTypeLookup";
        public static readonly string SQL_TABLES_ENTITY_PROPERTY_LOOKUP_TABLE = "EntityPropertyLookup";
        public static readonly string SQL_TABLES_HISTORY_HISTORYLOG = "HistoryLog";
        public static readonly string SQL_TABLES_HISTORY_ENTITYPROPERTYCHANGES = "EntityPropertyChanges";

        public static readonly string OPERATION_RESULT = "OperationResult";
    }
}
