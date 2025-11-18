using DirectoryService.Application.Commands.Departments.UpdateLocation;
using DirectoryService.Domain.Enities;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.IntegrationTests.Departments;

public class UpdateDepartmentLocationTests : CommandTestBase<UpdateDepartmentLocationCommand>
{
    public UpdateDepartmentLocationTests(DirectoryTestWebFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task UpdateDepartment_ValidData_Succeeds()
    {
        // Arrange
        var locationId = await CreateLocation("Локация1", "Москва, ул Королева 56, кв1");
        var departmentId = await CreateDepartment(locationId);
        var newLocationId = await CreateLocation("Локация2", "Москва, ул Королева 56, кв2");
        var command = new UpdateDepartmentLocationCommand(departmentId, [newLocationId]);

        // Act
        var result = await HandleCommand(command);

        // Assert
        var department = await ExecuteInDb<Department?>(async dbContext =>
            await dbContext.Departments
                .Include(d => d.DepartmentLocations)
                .FirstOrDefaultAsync(x => x.Id == departmentId));

        Assert.NotNull(department);
        Assert.Equal(department.Id, departmentId);
        Assert.True(result.IsSuccess);
        Assert.Equal([newLocationId], department.DepartmentLocations?.Select(x => x.LocationId));
    }

    [Fact]
    public async Task UpdateDepartment_DuplicateLocationIds_Fails()
    {
        // Arrange
        var locationId = await CreateLocation("Локация1", "Москва, ул Королева 56, кв1");
        var departmentId = await CreateDepartment(locationId);
        var newLocationId = await CreateLocation("Локация2", "Москва, ул Королева 56, кв2");
        var command = new UpdateDepartmentLocationCommand(departmentId, [newLocationId, newLocationId]);

        // Act
        var result = await HandleCommand(command);

        // Assert
        var department = await ExecuteInDb<Department?>(async dbContext =>
            await dbContext.Departments
                .Include(d => d.DepartmentLocations)
                .FirstOrDefaultAsync(x => x.Id == departmentId));

        Assert.NotNull(department);
        Assert.Equal(department.Id, departmentId);
        Assert.True(result.IsFailure);
        Assert.Equal("Collection 'LocationIds' contains duplicate elements", result.Error.Errors[0].Message);
        Assert.Equal([locationId], department.DepartmentLocations?.Select(x => x.LocationId));
    }


    private async Task<Guid> CreateDepartment(Guid locationId)
    {
        var department = FixtureExtensions.CreateDepartment(locationId);

        await ExecuteInDb(async context =>
        {
            context.Departments.Add(department);
            await context.SaveChangesAsync();
        });

        return department.Id;
    }

    private async Task<Guid> CreateLocation(string? name = null, string? address = null, string? timeZone = null)
    {
        var location = FixtureExtensions.CreateLocation(name, address, timeZone);

        await ExecuteInDb(async context =>
        {
            context.Locations.Add(location);
            await context.SaveChangesAsync();
        });

        return location.Id;
    }
}