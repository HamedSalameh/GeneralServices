using static GeneralServices.Enums;

namespace GeneralServices
{
    public abstract class BaseResult
    {
        public ResponseStatus Status { get; set; }
        public string Message { get; set; }
        public object Value { get; set; }
        public OperationType Type { get; set; }
    }
    public class ValidationResult : BaseResult
    {
        public string PropertyName { get; set; }

        public ValidationResult() : 
            this(ResponseStatus.Unknown, string.Empty)
        {
        }

        public ValidationResult(string PropertyName) : 
            this(ResponseStatus.Unknown, PropertyName)
        {
        }

        public ValidationResult(ResponseStatus ValidationStatus, string PropertyName) 
        {
            this.PropertyName = PropertyName;
            Status = ValidationStatus;
            Type = OperationType.Validation;
        }
    }

    public class OperationResult : BaseResult
    {
        public string OperationName { get; set; }
        public OperationResult() : this(ResponseStatus.Unknown, string.Empty)
        {
        }
        public OperationResult(string OperationName) : this(ResponseStatus.Unknown, OperationName)
        {
        }

        public OperationResult(ResponseStatus OperationStatus, string OperationName)
        {
            this.OperationName = OperationName;
            Status = OperationStatus;
            Type = OperationType.Operation;
        }
    }
}
