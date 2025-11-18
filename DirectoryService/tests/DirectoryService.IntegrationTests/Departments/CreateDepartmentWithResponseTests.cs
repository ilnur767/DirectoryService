using DirectoryService.Application.Commands.Departments.CreateDepartment;
using DirectoryService.Domain.Enities;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.IntegrationTests.Departments;

public class CreateDepartmentWithResponseTests : CommandWithResponseTestBase<Guid, CreateDepartmentCommand>
{
    public CreateDepartmentWithResponseTests(DirectoryTestWebFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateDepartment_ValidData_Succeeds()
    {
        // Arrange
        var locationId = await CreateLocation();

        var command = new CreateDepartmentCommand(
            "Подразделение",
            "podrazdelenie",
            null,
            [locationId]);

        // Act
        var result = await HandleCommand(command);

        // Assert
        var department = await ExecuteInDb<Department?>(async dbContext =>
            await dbContext.Departments.FirstOrDefaultAsync(x => x.Id == result.Value));

        Assert.NotNull(department);
        Assert.Equal(department.Id, result.Value);
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
    }

    [Theory]
    [InlineData(151)]
    [InlineData(0)]
    [InlineData(2)]
    public async Task CreateDepartment_InValidDepartmentName_Fails(int length)
    {
        // Arrange
        var locationId = await CreateLocation();

        var command = new CreateDepartmentCommand(
            new string('d', length),
            "podrazdelenie",
            null,
            [locationId]);

        // Act
        var result = await HandleCommand(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("DepartmentName is invalid", result.Error.Errors[0].Message);
    }

    [Theory]
    [InlineData(151)]
    [InlineData(0)]
    [InlineData(2)]
    public async Task CreateDepartment_InValidIdentifier_Fails(int length)
    {
        // Arrange
        var locationId = await CreateLocation();

        var command = new CreateDepartmentCommand(
            "Подразделение",
            new string('i', length),
            null,
            [locationId]);

        // Act
        var result = await HandleCommand(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Identifier is invalid", result.Error.Errors[0].Message);
    }

    [Fact]
    public async Task CreateDepartment_DuplicateLocationIds_Fails()
    {
        // Arrange
        var locationId = await CreateLocation();

        var command = new CreateDepartmentCommand(
            "Подразделение",
            "podrazdelenie",
            null,
            [locationId, locationId]);

        // Act
        var result = await HandleCommand(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Collection 'LocationIds' contains duplicate elements", result.Error.Errors[0].Message);
    }

    private async Task<Guid> CreateLocation()
    {
        var location = FixtureExtensions.CreateLocation();

        await ExecuteInDb(async context =>
        {
            context.Locations.Add(location);
            await context.SaveChangesAsync();
        });

        return location.Id;
    }
}