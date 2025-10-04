using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared.Errors;
using static DirectoryService.Domain.Constants.LocationConstants;

namespace DirectoryService.Domain.ValueObjects;

public record Address
{
    private const string PATTERN = "^[\\p{L}0-9\\s.,\\-_/\\\\#()]+$";

    private Address(string value) => Value = value;

    private Address() { }

    public string Value { get; }

    public Result<Address, Error> Create(string name)
    {
        if (name.Length < ADDRESS_MIN_LENGTH || name.Length > ADDRESS_MAX_LENGTH)
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