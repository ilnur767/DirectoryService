using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared.Errors;
using static DirectoryService.Domain.Constants.DepartmentConstants;

namespace DirectoryService.Domain.ValueObjects;

public record Identifier
{
    private const string PATTERN = "^[A-Za-z]$";

    private Identifier(string value) => Value = value;
    private Identifier() { }

    public string Value { get; }

    public static Result<Identifier, Error> Create(string identifier)
    {
        if (identifier.Length < IDENTIFIER_MIN_LENGTH || identifier.Length > IDENTIFIER_MAX_LENGTH)
        {
            return Errors.General.ValueIsInvalid(nameof(DepartmentName));
        }

        if (!Regex.IsMatch(identifier, PATTERN))
        {
            return Errors.General.ValueIsInvalid(nameof(Identifier));
        }

        return new Identifier(identifier);
    }
}