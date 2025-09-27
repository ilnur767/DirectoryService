using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared.Errors;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Domain.Enitties;

/// <summary>
///     Представляет подразделение организации.
///     Содержит информацию о названии, идентификаторе, родительском подразделении, глубине и пути в иерархии, а также
///     статус активности.
/// </summary>
public class Department
{
    private List<DepartmentLocation>? _departmentLocations;

    private List<DepartmentPosition>? _departmentPositions;

    private Department(Guid id,
        DepartmentName name,
        Identifier identifier,
        Department? parent,
        DepartmentPath path,
        short depth,
        bool isActive,
        DateTime createdAt)
    {
        Id = id;
        Name = name;
        Identifier = identifier;
        Parent = parent;
        Depth = depth;
        Path = path;
        IsActive = isActive;
        CreatedAt = createdAt;
    }

    public Guid Id { get; }
    public DepartmentName Name { get; private set; }
    public Identifier Identifier { get; private set; }
    public Department? Parent { get; private set; }
    public DepartmentPath Path { get; private set; }
    public short Depth { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyList<DepartmentPosition>? DepartmentPositions => _departmentPositions;

    public IReadOnlyList<DepartmentLocation>? DepartmentLocations => _departmentLocations;

    public static Result<Department, Error> Create(Department? parent, DepartmentName name, Identifier identifier)
    {
        DateTime createdAt = DateTime.Now;

        short depth = (short)(parent == null ? 0 : parent.Depth + 1);

        string path = parent == null ? identifier.Value : $"{identifier.Value}{parent.Path}";

        Result<DepartmentPath, Error> departmentPath = DepartmentPath.Create(path);

        if (departmentPath.IsFailure)
        {
            return departmentPath.Error;
        }

        return new Department(Guid.NewGuid(), name, identifier, parent, departmentPath.Value, depth, true, createdAt);
    }

    public void AddPosition(IEnumerable<Position> positions)
    {
        List<DepartmentPosition> departmentPositions =
            positions.Select(p => new DepartmentPosition(this, p, Id, p.Id)).ToList();

        _departmentPositions.AddRange(departmentPositions);
    }

    public void AddLocation(IEnumerable<Location> locations)
    {
        List<DepartmentLocation> departmentLocations =
            locations.Select(l => new DepartmentLocation(this, l, Id, l.Id)).ToList();

        _departmentLocations.AddRange(departmentLocations);
    }

    public void UpdateName(DepartmentName name)
    {
        Name = name;
        UpdateTimestamp();
    }

    public UnitResult<Error> UpdateIdentifier(Identifier identifier)
    {
        string path = Parent == null
            ? identifier.Value
            : $"{Parent.Path}.{identifier.Value}";

        Result<DepartmentPath, Error> departmentPath = DepartmentPath.Create(path);
        if (departmentPath.IsFailure)
        {
            return departmentPath.Error;
        }

        Path = departmentPath.Value;

        Identifier = identifier;

        UpdateTimestamp();

        return UnitResult.Success<Error>();
    }

    public Result<Department, Error> ChangeParent(Department parent)
    {
        if (parent == this)
        {
            return Errors.Department.CannotAssignSelfAsParent();
        }

        Parent = parent;

        short depth = (short)(parent.Depth + 1);
        Depth = depth;

        string path = $"{parent.Path}.{Identifier.Value}";

        Result<DepartmentPath, Error> departmentPath = DepartmentPath.Create(path);
        if (departmentPath.IsFailure)
        {
            return departmentPath.Error;
        }

        Path = departmentPath.Value;
        UpdateTimestamp();
        return this;
    }

    public void Delete()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        UpdateTimestamp();
    }

    private void UpdateTimestamp() => UpdatedAt = DateTime.UtcNow;
}