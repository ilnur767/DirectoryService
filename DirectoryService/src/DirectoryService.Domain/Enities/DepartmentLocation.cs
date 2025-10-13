namespace DirectoryService.Domain.Enities;

public class DepartmentLocation
{
    public DepartmentLocation(Department department, Location location, Guid departmentId, Guid locationId)
    {
        Department = department;
        Location = location;
        DepartmentId = departmentId;
        LocationId = locationId;
        Id = Guid.NewGuid();
    }

    public DepartmentLocation() { }

    public Guid Id { get; private set; }
    public Department Department { get; private set; }
    public Guid DepartmentId { get; }
    public Location Location { get; private set; }
    public Guid LocationId { get; private set; }
}