namespace DirectoryService.Contracts.Departments;

public class DepartmentByPositionDto
{
    public Guid Id { get; }
    public string Name { get; private set; } = null!;
    public string Identifier { get; private set; } = null!;
    public string Path { get; private set; } = null!;
    public short Depth { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public int PositionsCount { get; private set; }
}