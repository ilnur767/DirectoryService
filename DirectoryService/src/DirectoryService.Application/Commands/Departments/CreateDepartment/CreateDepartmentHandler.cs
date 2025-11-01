using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Domain.Enities;
using DirectoryService.Domain.Shared.Errors;
using DirectoryService.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Commands.Departments.CreateDepartment;

public sealed class CreateDepartmentHandler : ICommandHandler<Guid, CreateDepartmentCommand>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<CreateDepartmentHandler> _logger;

    public CreateDepartmentHandler(
        IDepartmentRepository departmentRepository,
        ILogger<CreateDepartmentHandler> logger, ILocationRepository locationRepository)
    {
        _departmentRepository = departmentRepository;
        _logger = logger;
        _locationRepository = locationRepository;
    }

    public async Task<Result<Guid, ErrorList>> Handle(CreateDepartmentCommand command,
        CancellationToken cancellationToken)
    {
        Department? parent = null;
        if (command.ParentId != null)
        {
            var parentDepartment =
                await _departmentRepository.GetDepartmentById(command.ParentId.Value, cancellationToken);

            if (parentDepartment.IsFailure)
            {
                return parentDepartment.Error.ToErrorList();
            }

            parent = parentDepartment.Value;
        }

        var departmentId = Guid.NewGuid();

        var departmentName = DepartmentName.Create(command.Name).Value;

        if (parent != null && parent.Path.Value.Contains(command.Identifier))
        {
            return Errors.General.ValueIsInvalid(nameof(command.Identifier)).ToErrorList();
        }

        var identifier = Identifier.Create(command.Identifier).Value;

        var isAllLocationsExists =
            await _locationRepository.CheckIfAllLocationsExist(command.LocationIds, cancellationToken);

        if (isAllLocationsExists.IsFailure)
        {
            return isAllLocationsExists.Error.ToErrorList();
        }

        var departmentLocations = command.LocationIds.Select(l => new DepartmentLocation(departmentId, l)).ToList();

        var department = Department.Create(departmentId, parent, departmentName, identifier, departmentLocations);

        if (department.IsFailure)
        {
            _logger.LogError("Could not create department [{name}]", command.Name);
            return department.Error.ToErrorList();
        }

        var result = await _departmentRepository.CreateDepartment(department.Value, cancellationToken);
        if (result.IsFailure)
        {
            return result.Error.ToErrorList();
        }
        
        _logger.LogInformation("Created department with id '{id}'", result.Value);

        return result.Value;
    }
}

public record CreateDepartmentCommand(string Name, string Identifier, Guid? ParentId, Guid[] LocationIds) : ICommand;