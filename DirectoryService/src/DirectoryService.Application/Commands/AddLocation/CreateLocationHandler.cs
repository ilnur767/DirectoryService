using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Domain.Enities;
using DirectoryService.Domain.Shared.Errors;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Application.Commands.AddLocation;

public class CreateLocationHandler : ICommandHandler<Guid, AddLocationCommand>
{
    private readonly ILocationRepository _locationRepository;

    public CreateLocationHandler(ILocationRepository locationRepository) => _locationRepository = locationRepository;

    public async Task<Result<Guid, ErrorList>> Handle(AddLocationCommand command, CancellationToken cancellationToken)
    {
        var address = Address.Create(command.Address);
        if (address.IsFailure)
        {
            return address.Error.ToErrorList();
        }

        var locationName = LocationName.Create(command.Name);
        if (locationName.IsFailure)
        {
            return locationName.Error.ToErrorList();
        }

        var timeZone = Timezone.Create(command.TimeZone);
        if (timeZone.IsFailure)
        {
            return timeZone.Error.ToErrorList();
        }

        var location = Location.Create(locationName.Value, address.Value, timeZone.Value);

        var result = await _locationRepository.CreateLocation(location, cancellationToken);
        if (result.IsFailure)
        {
            return result.Error.ToErrorList();
        }

        return location.Id;
    }
}

public record AddLocationCommand(string Name, string Address, string TimeZone) : ICommand;