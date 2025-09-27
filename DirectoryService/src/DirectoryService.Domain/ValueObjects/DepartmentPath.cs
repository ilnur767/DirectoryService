using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared.Errors;

namespace DirectoryService.Domain.ValueObjects;

public record DepartmentPath
{
    private const string PATTERN = @"^[a-z]+(\.[a-z0-9-]+)*$";

    private DepartmentPath(string path) => Value = path;

    public string Value { get; private set; }

    public static Result<DepartmentPath, Error> Create(string path)
    {
        if (Regex.IsMatch(path, PATTERN))
        {
            return Errors.General.ValueIsInvalid(nameof(DepartmentPath));
        }

        return new DepartmentPath(path);
    }
}