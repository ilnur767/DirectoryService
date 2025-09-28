using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared.Errors;

namespace DirectoryService.Domain.ValueObjects;

public record Identifier
{
    private const int MIN_LENGTH = 3;
    private const int MAX_LENGTH = 150;
    private const string PATTERN = "^[A-Za-z]$";

    private Identifier(string path) => Value = path;

    public string Value { get; }

    public static Result<Identifier, Error> Create(string identifier)
    {
        if (identifier.Length < MIN_LENGTH || identifier.Length > MAX_LENGTH)
        {
            return Errors.General.ValueIsInvalid(nameof(DepartmentName));
        }

        if (Regex.IsMatch(identifier, PATTERN))
        {
            return Errors.General.ValueIsInvalid(nameof(Identifier));
        }

        return new Identifier(identifier);
    }
}