using DirectoryService.Domain.Enities;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.IntegrationTests;

public static class FixtureExtensions
{
    public static Department CreateDepartment(Guid locationId, Department? parent = null)
    {
        var departmentId = Guid.NewGuid();
        var departmentName = DepartmentName.Create("Подразделение").Value;
        var identifier = Identifier.Create("podrazdelenie").Value;
        var department = Department.Create(departmentId, parent, departmentName, identifier,
            [new DepartmentLocation(departmentId, locationId)]).Value;

        return department;
    }

    public static Location CreateLocation(string? name = null, string? address = null, string? timeZone = null)
    {
        var location = Location.Create(
            LocationName.Create(name ?? "Локация").Value,
            Address.Create(address ?? "Москва, ул. Ленина 145, д.29").Value,
            Timezone.Create(timeZone ?? "Europe/Moscow").Value);

        return location;
    }
}