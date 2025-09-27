using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared.Errors;

namespace DirectoryService.Domain.ValueObjects;

public record Address
{
    private const string PATTERN = "^[\\p{L}0-9\\s.,\\-_/\\\\#()]+$";
    private const int MIN_LENGTH = 3;
    private const int MAX_LENGTH = 500;

    private Address(string name) => Value = name;

    public string Value { get; private set; }

    public Result<Address, Error> Create(string name)
    {
        if (name.Length < MIN_LENGTH || name.Length > MAX_LENGTH)
        {
            return Errors.General.ValueIsInvalid(nameof(Address));
        }

        if (Regex.IsMatch(name, PATTERN))
        {
            return Errors.General.ValueIsInvalid(nameof(Address));
        }

        return new Address(name);
    }
}