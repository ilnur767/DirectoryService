namespace DirectoryService.Domain.Shared.Errors;

public static class Errors
{
    public const string INVALID_VALUE_CODE = "value.is.invalid";
    public const string RECORD_NOT_FOUND_CODE = "record.not.found";

    public static class General
    {
        public static Error ValueIsInvalid(string? name = null)
        {
            string label = name ?? "value";

            return Error.Validation(INVALID_VALUE_CODE, $"{label} is invalid");
        }

        public static Error NotFound(Guid? id = null)
        {
            string forId = id == null ? "" : $" for Id '{id}'";

            return Error.NotFound(RECORD_NOT_FOUND_CODE, $"record not found{forId}");
        }

        public static Error NotFound(string message) => Error.NotFound(RECORD_NOT_FOUND_CODE, message);

        public static Error ValueIsRequired(string? name = null)
        {
            string label = name == null ? "" : " " + name + " ";

            return Error.Validation(INVALID_VALUE_CODE, $"invalid{label}length");
        }

        public static Error AlreadyExists() => Error.NotFound("record.already.exists", "Record already exists");

        public static Error Failure() => Error.Failure("failure", "failure");
    }

    public static class Department
    {
        public static Error CannotAssignSelfAsParent() => Error.NotFound("department.assign.parent",
            "Cannot assign the department as its own parent.");
    }
}