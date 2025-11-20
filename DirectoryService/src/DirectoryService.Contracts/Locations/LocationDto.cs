namespace DirectoryService.Contracts.Locations;

public class LocationDto
{
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Timezone { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}