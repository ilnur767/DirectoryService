using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared.Errors;
using static DirectoryService.Domain.Constants.DepartmentConstants;

namespace DirectoryService.Domain.ValueObjects;

public record DepartmentName
{
    private DepartmentName(string value) => Value = value;
    private DepartmentName() { }
    public string Value { get; }


    public Result<DepartmentName, Error> Create(string name)
    {
        if (name.Length < NAME_MIN_LENGTH || name.Length > NAME_MAX_LENGTH)
        {
            return Errors.General.ValueIsInvalid(nameof(DepartmentName));
        }

        return new DepartmentName(name);
    }
}