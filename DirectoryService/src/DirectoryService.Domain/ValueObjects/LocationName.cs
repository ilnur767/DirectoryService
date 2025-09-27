using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared.Errors;

namespace DirectoryService.Domain.ValueObjects;

public record LocationName
{
    private const int MIN_LENGTH = 3;
    private const int MAX_LENGTH = 120;

    private LocationName(string name) => Value = name;

    public string Value { get; }

    public Result<LocationName, Error> Create(string name)
    {
        if (name.Length < MIN_LENGTH || name.Length > MAX_LENGTH)
        {
            return Errors.General.ValueIsInvalid(nameof(LocationName));
        }

        return new LocationName(name);
    }
}