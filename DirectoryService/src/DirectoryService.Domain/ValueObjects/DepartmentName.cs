using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared.Errors;

namespace DirectoryService.Domain.ValueObjects;

public record DepartmentName
{
    private const int MIN_LENGTH = 3;
    private const int MAX_LENGTH = 150;

    private DepartmentName(string path) => Value = path;

    public string Value { get; private set; }


    public Result<DepartmentName, Error> Create(string name)
    {
        if (name.Length < MIN_LENGTH || name.Length > MAX_LENGTH)
        {
            return Errors.General.ValueIsInvalid(nameof(DepartmentName));
        }

        return new DepartmentName(name);
    }
}