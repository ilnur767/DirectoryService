using DirectoryService.Domain.Shared.Errors;
using FluentValidation.Results;

namespace DirectoryService.Application.Extensions;

public static class ValidationExtensions
{
    public static ErrorList ToErrorList(this ValidationResult validationResult)
    {
        if (validationResult.IsValid)
        {
            throw new InvalidOperationException("Result cannot be successfully validated.");
        }

        var validationErrors = validationResult.Errors;

        var errors = validationErrors.Select(v => Error.Serialize(v.ErrorMessage));

        return new ErrorList(errors);
    }
}