namespace GeneralServices
{
    public static class Enums
    {
        public enum MonthNames
        {
            January = 1,
            February,
            March,
            April,
            May,
            June,
            July,
            August,
            September,
            October,
            November,
            December
        }

        public enum OperationType
        {
            Operation,
            Validation
        }

        public enum ResponseStatus
        {
            Unknown,
            Success,
            Exception,
            Failure
        }

        public enum CreditCardType
        { 
            VISA,
            MasterCard
        }

        public enum CRUDType
        {
            Create,
            Update,
            Delete
        }

        public enum SQL_TransactionCommands
        {
            Begin = 0,
            Commit,
            Rollback
        }
    }
}
