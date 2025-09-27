using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared.Errors;

namespace DirectoryService.Domain.Enitties;

/// <summary>
///     Представляет должность (Position) сотрудника.
///     Содержит название, описание, статус активности и информацию о создании/обновлении.
/// </summary>
public class Position
{
    private const int MIN_NAME_LENGTH = 3;
    private const int MAX_NAME_LENGTH = 100;
    private const int MAX_DESCRIPTION_LENGTH = 1000;

    private List<DepartmentPosition> _departmentPositions;

    private Position()
    {
    }

    private Position(string name, string? description, IEnumerable<DepartmentPosition> departmentPositions)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        DepartmentPositions = departmentPositions.ToList();
        UpdatedAt = CreatedAt;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyList<DepartmentPosition> DepartmentPositions
    {
        get => _departmentPositions;
        private set => _departmentPositions = value.ToList();
    }

    public static Result<Position, Error> Create(string name, string? description,
        IEnumerable<DepartmentPosition> departmentPositions)
    {
        UnitResult<Error> nameResult = CheckName(name);
        if (nameResult.IsFailure)
        {
            return nameResult.Error;
        }

        UnitResult<Error> postionResult = CheckDescription(name);
        if (postionResult.IsFailure)
        {
            return postionResult.Error;
        }

        return new Position(name, description, departmentPositions);
    }

    private static UnitResult<Error> CheckName(string name)
    {
        if (name.Length < MIN_NAME_LENGTH || name.Length > MAX_NAME_LENGTH)
        {
            return Errors.General.ValueIsInvalid(nameof(name));
        }

        return UnitResult.Success<Error>();
    }

    private static UnitResult<Error> CheckDescription(string description)
    {
        if (description.Length > MAX_DESCRIPTION_LENGTH)
        {
            return Errors.General.ValueIsInvalid(nameof(description));
        }

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> UpdateName(string name)
    {
        UnitResult<Error> result = CheckName(name);
        if (result.IsFailure)
        {
            return result.Error;
        }

        Name = name;
        UpdateTimestamp();

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> UpdateDescription(string description)
    {
        UnitResult<Error> result = CheckDescription(description);
        if (result.IsFailure)
        {
            return result.Error;
        }

        Description = description;
        UpdateTimestamp();

        return UnitResult.Success<Error>();
    }

    public void Delete()
    {
        if (IsActive)
        {
            IsActive = false;
            UpdateTimestamp();
        }
    }

    private void UpdateTimestamp() => UpdatedAt = DateTime.UtcNow;
}