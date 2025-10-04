using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared.Errors;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Domain.Enitties;

public class Location
{
    private readonly List<DepartmentLocation> _departmentLocations;
    private Location() { }

    private Location(Guid id, LocationName name, Address address, Timezone timezone)
    {
        Id = id;
        Name = name;
        Address = address;
        Timezone = timezone;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public LocationName Name { get; private set; }
    public Address Address { get; private set; }
    public Timezone Timezone { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyList<DepartmentLocation>? DepartmentLocations => _departmentLocations;

    public static Location Create(LocationName name, Address address, Timezone timezone)
    {
        Guid id = Guid.NewGuid();

        return new Location(id, name, address, timezone);
    }

    public UnitResult<Error> AddDepartmentLocation(DepartmentLocation departmentLocation)
    {
        _departmentLocations.Add(departmentLocation);
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