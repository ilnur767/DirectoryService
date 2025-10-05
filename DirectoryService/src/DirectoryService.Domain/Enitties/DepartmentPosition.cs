namespace DirectoryService.Domain.Enitties;

public class DepartmentPosition
{
    public DepartmentPosition(Department department, Position position, Guid departmentId, Guid positionId)
    {
        Department = department;
        Position = position;
        DepartmentId = departmentId;
        PositionId = positionId;
        Id = Guid.NewGuid();
    }

    public DepartmentPosition() { }

    public Guid Id { get; private set; }
    public Department Department { get; private set; }
    public Guid DepartmentId { get; }
    public Position Position { get; private set; }
    public Guid PositionId { get; }
}