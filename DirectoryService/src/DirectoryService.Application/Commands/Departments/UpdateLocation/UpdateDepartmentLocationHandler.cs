using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Domain.Shared.Errors;

namespace DirectoryService.Application.Commands.Departments.UpdateLocation;

public sealed class UpdateDepartmentLocationHandler : ICommandHandler<UpdateDepartmentLocationCommand>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILocationRepository _locationRepository;

    public UpdateDepartmentLocationHandler(
        IDepartmentRepository departmentRepository,
        ILocationRepository locationRepository)
    {
        _departmentRepository = departmentRepository;
        _locationRepository = locationRepository;
    }

    public async Task<UnitResult<ErrorList>> Handle(UpdateDepartmentLocationCommand command,
        CancellationToken cancellationToken)
    {
        var existsDepartments = await _departmentRepository.GetDepartmentById(command.DepartmentId, cancellationToken);

        if (existsDepartments.IsFailure)
        {
            return existsDepartments.Error.ToErrorList();
        }

        if (existsDepartments.Value == null)
        {
            return Errors.General.NotFound(command.DepartmentId).ToErrorList();
        }

        var locationsExist = await _locationRepository.CheckIfAllLocationsExist(command.LocationIds, cancellationToken);
        if (locationsExist.IsFailure)
        {
            return locationsExist.Error.ToErrorList();
        }

        if (!locationsExist.Value)
        {
            return Errors.General.NotFound("Not all locations exist").ToErrorList();
        }

        var department = existsDepartments.Value;
        department.AddLocations(command.LocationIds);

        var result = await _departmentRepository.Save(cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToErrorList();
        }

        return UnitResult.Success<ErrorList>();
    }
}

public record UpdateDepartmentLocationCommand(Guid DepartmentId, Guid[] LocationIds) : ICommand;