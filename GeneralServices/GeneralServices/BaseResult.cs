using static GeneralServices.Enums;

namespace GeneralServices
{
    public abstract class BaseResult
    {
        public ResultStatus Status { get; set; }
        public string Message { get; set; }
        public object Value { get; set; }
        public OperationType Type { get; set; }
    }
    public class ValidationResult : BaseResult
    {
        public string PropertyName { get; set; }

        public ValidationResult() : 
            this(ResultStatus.Unknown, string.Empty)
        {
        }

        public ValidationResult(string PropertyName) : 
            this(ResultStatus.Unknown, PropertyName)
        {
        }

        public ValidationResult(ResultStatus ValidationStatus, string PropertyName) 
        {
            this.PropertyName = PropertyName;
            Status = ValidationStatus;
            Type = OperationType.Validation;
        }
    }

    public class OperationResult : BaseResult
    {
        public string OperationName { get; set; }
        public OperationResult() : this(ResultStatus.Unknown, string.Empty)
        {
        }
        public OperationResult(string OperationName) : this(ResultStatus.Unknown, OperationName)
        {
        }

        public OperationResult(ResultStatus OperationStatus, string OperationName)
        {
            this.OperationName = OperationName;
            Status = OperationStatus;
            Type = OperationType.Operation;
        }
    }
}
