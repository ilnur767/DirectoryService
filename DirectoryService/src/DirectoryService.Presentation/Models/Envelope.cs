using DirectoryService.Domain.Shared.Errors;

namespace DirectoryService.Presentation.Models;

public record ResponseError(string? ErrorCode, string? ErrorMessage, string? InvalidField);

public record Envelop
{
    private Envelop(object? result, ErrorList errors)
    {
        Result = result;
        Errors = errors;
        CreatedAt = DateTime.UtcNow;
    }

    public object? Result { get; }

    public ErrorList? Errors { get; }

    public DateTimeOffset CreatedAt { get; }

    public static Envelop Ok(object? result) => new(result, null);

    public static Envelop Error(ErrorList errors) => new(null, errors);

    public static Envelop Error(Error error) => new(null, error.ToErrorList());
}