using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Domain.Enities;
using DirectoryService.Domain.Shared.Errors;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Commands.Positions.CreatePosition;

public sealed class CreatePositionHandler : ICommandHandler<Guid, CreatePositionCommand>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogger<CreatePositionHandler> _logger;
    private readonly IPositionRepository _positionRepository;

    public CreatePositionHandler(
        IPositionRepository positionRepository,
        IDepartmentRepository departmentRepository,
        ILogger<CreatePositionHandler> logger)
    {
        _positionRepository = positionRepository;
        _departmentRepository = departmentRepository;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(CreatePositionCommand command,
        CancellationToken cancellationToken)
    {
        var departments =
            await _departmentRepository.CheckIfAllDepartmentsExist(command.DepartmentIds, cancellationToken);
        if (departments.IsFailure)
        {
            return departments.Error.ToErrorList();
        }

        var positionId = Guid.NewGuid();

        var position = Position.Create(
            positionId,
            command.Name,
            command.Description,
            command.DepartmentIds.Select(d => new DepartmentPosition(d, positionId)));

        if (position.IsFailure)
        {
            return position.Error.ToErrorList();
        }

        var result = await _positionRepository.CreatePosition(position.Value, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("Could not create position [{name}]", command.Name);

            return result.Error.ToErrorList();
        }
        
        _logger.LogInformation("Created position with id '{id}'", result.Value);

        return result.Value;
    }
}

public record CreatePositionCommand(string Name, string Description, Guid[] DepartmentIds) : ICommand;