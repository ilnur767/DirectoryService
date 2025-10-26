using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Domain.Enities;
using DirectoryService.Domain.Shared.Errors;
using DirectoryService.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Commands.AddLocation;

public class CreateLocationHandler : ICommandHandler<Guid, AddLocationCommand>
{
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<CreateLocationHandler> _logger;

    public CreateLocationHandler(ILocationRepository locationRepository, ILogger<CreateLocationHandler> logger)
    {
        _locationRepository = locationRepository;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(AddLocationCommand command, CancellationToken cancellationToken)
    {
        var address = Address.Create(command.Address);
        if (address.IsFailure)
        {
            _logger.LogError("Location address validation failed: {error}", address.Error);

            return address.Error.ToErrorList();
        }

        var locationName = LocationName.Create(command.Name);
        if (locationName.IsFailure)
        {
            _logger.LogError("Location name validation failed: {error}", locationName.Error);

            return locationName.Error.ToErrorList();
        }

        var timeZone = Timezone.Create(command.TimeZone);
        if (timeZone.IsFailure)
        {
            _logger.LogError("Location timezone validation failed: {error}", timeZone.Error);

            return timeZone.Error.ToErrorList();
        }

        var location = Location.Create(locationName.Value, address.Value, timeZone.Value);

        var result = await _locationRepository.CreateLocation(location, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("Location timezone validation failed: {error}", result.Error);

            return result.Error.ToErrorList();
        }

        _logger.LogInformation("Created location with id '{id}'", result.Value);
        return location.Id;
    }
}

public record AddLocationCommand(string Name, string Address, string TimeZone) : ICommand;