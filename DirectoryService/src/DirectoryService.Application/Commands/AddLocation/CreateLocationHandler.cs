using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Domain.Enities;
using DirectoryService.Domain.Shared.Errors;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Commands.AddLocation;

public class CreateLocationHandler : ICommandHandler<Guid, AddLocationCommand>
{
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<CreateLocationHandler> _logger;

    public CreateLocationHandler(
        ILocationRepository locationRepository,
        ILogger<CreateLocationHandler> logger,
        IValidator<AddLocationCommand> validator)
    {
        _locationRepository = locationRepository;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(AddLocationCommand command, CancellationToken cancellationToken)
    {
        var address = Address.Create(command.Address);

        var locationName = LocationName.Create(command.Name);

        var timeZone = Timezone.Create(command.TimeZone);

        var location = Location.Create(locationName.Value, address.Value, timeZone.Value);

        var result = await _locationRepository.CreateLocation(location, cancellationToken);

        _logger.LogInformation("Created location with id '{id}'", result.Value);
        return location.Id;
    }
}

public record AddLocationCommand(string Name, string Address, string TimeZone) : ICommand;