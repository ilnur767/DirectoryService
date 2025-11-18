using DirectoryService.Application.Commands.Departments.MoveDepartment;
using DirectoryService.Domain.Enities;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.IntegrationTests.Departments;

public class MoveDepartmentTests : CommandTestBase<MoveDepartmentCommand>
{
    public MoveDepartmentTests(DirectoryTestWebFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task MoveDepartment_ValidData_Succeeds()
    {
        //Arrange
        var location1 = FixtureExtensions.CreateLocation("Локация1", "Москва, ул. Ленина 145, д.29");
        var location2 = FixtureExtensions.CreateLocation("Локация2", "Москва, ул. Ленина 145, д.30");
        var location3 = FixtureExtensions.CreateLocation("Локация3", "Москва, ул. Ленина 145, д.31");
        await ExecuteInDb(async context =>
        {
            context.Locations.AddRange(location1, location2, location3);
            await context.SaveChangesAsync();
        });

        var department = FixtureExtensions.CreateDepartment(location1.Id);
        var childDepartment = FixtureExtensions.CreateDepartment(location2.Id, department);

        var anotherDepartment = FixtureExtensions.CreateDepartment(location3.Id);

        await ExecuteInDb(async context =>
        {
            context.Departments.AddRange(department, childDepartment, anotherDepartment);
            await context.SaveChangesAsync();
        });

        //Act
        var result = await HandleCommand(new MoveDepartmentCommand(childDepartment.Id, anotherDepartment.Id));

        //Assert
        var newChildDepartment = await ExecuteInDb<Department?>(async dbContext =>
            await dbContext.Departments
                .Include(d => d.Parent)
                .FirstOrDefaultAsync(x => x.Id == childDepartment.Id));

        Assert.True(result.IsSuccess);
        Assert.Equal($"{anotherDepartment.Path.Value}.{newChildDepartment?.Identifier.Value}",
            newChildDepartment?.Path.Value);
        Assert.Equal(anotherDepartment.Id, newChildDepartment?.Parent?.Id);
    }

    [Fact]
    public async Task MoveDepartment_AssignToChild_Fails()
    {
        //Arrange
        var location1 = FixtureExtensions.CreateLocation("Локация1", "Москва, ул. Ленина 145, д.29");
        var location2 = FixtureExtensions.CreateLocation("Локация2", "Москва, ул. Ленина 145, д.30");
        var location3 = FixtureExtensions.CreateLocation("Локация3", "Москва, ул. Ленина 145, д.31");
        await ExecuteInDb(async context =>
        {
            context.Locations.AddRange(location1, location2, location3);
            await context.SaveChangesAsync();
        });

        var department = FixtureExtensions.CreateDepartment(location1.Id);
        var childDepartment = FixtureExtensions.CreateDepartment(location2.Id, department);

        var anotherDepartment = FixtureExtensions.CreateDepartment(location3.Id);

        await ExecuteInDb(async context =>
        {
            context.Departments.AddRange(department, childDepartment, anotherDepartment);
            await context.SaveChangesAsync();
        });

        //Act
        var result = await HandleCommand(new MoveDepartmentCommand(department.Id, childDepartment.Id));

        //Assert
        var updatedDepartment = await ExecuteInDb<Department?>(async dbContext =>
            await dbContext.Departments
                .Include(d => d.Parent)
                .FirstOrDefaultAsync(x => x.Id == department.Id));

        Assert.True(result.IsFailure);
        Assert.Equal("A department cannot be assigned as a parent to itself or any of its descendants.",
            result.Error.Errors[0].Message);
        Assert.Equal(department?.Parent?.Id, updatedDepartment?.Parent?.Id);
    }
}