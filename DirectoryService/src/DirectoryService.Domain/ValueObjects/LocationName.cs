using CSharpFunctionalExtensions;
using DirectoryService.Domain.Constants;
using DirectoryService.Domain.Shared.Errors;

namespace DirectoryService.Domain.ValueObjects;

using static LocationConstants;

public record LocationName
{
    private LocationName(string value) => Value = value;
    private LocationName() { }
    public string Value { get; }

    public static Result<LocationName, Error> Create(string name)
    {
        if (name.Length < NAME_MIN_LENGTH || name.Length > NAME_MAX_LENGTH)
        {
            return Errors.General.ValueIsInvalid(nameof(LocationName));
        }

        return new LocationName(name);
    }
}