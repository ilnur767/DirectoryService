namespace DirectoryService.Domain.Shared.Errors;

public record Error
{
    private const string Separator = "||";

    private Error(string code, string message, ErrorType type)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    public string Code { get; }
    public string Message { get; set; }
    public ErrorType Type { get; set; }

    public static Error Validation(string code, string message) => new(code, message, ErrorType.Validation);

    public static Error NotFound(string code, string message) => new(code, message, ErrorType.NotFound);

    public static Error Failure(string code, string message) => new(code, message, ErrorType.Failure);

    public static Error Conflict(string code, string message) => new(code, message, ErrorType.Conflict);

    public string Serialize() => string.Join(Separator, Code, Message, Type);

    public static Error Serialize(string serialized)
    {
        string[] parts = serialized.Split(Separator);

        if (parts.Length < 2)
        {
            throw new ArgumentException("Invalid serialized format.", serialized);
        }

        if (Enum.TryParse<ErrorType>(parts[2], out ErrorType type) == false)
        {
            throw new ArgumentException("Invalid serialized format.", serialized);
        }

        return new Error(parts[0], parts[1], type);
    }

    public ErrorList ToErrorList() => new([this]);
}

public enum ErrorType
{
    Validation,
    NotFound,
    Failure,
    Conflict
}

public class ErrorList
{
    public ErrorList(IEnumerable<Error> errors) => Errors = [..errors];

    public List<Error> Errors { get; }

    public static implicit operator ErrorList(Error error) => new([error]);
}