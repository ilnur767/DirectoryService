using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Commands.Locations.CreateLocation;
using DirectoryService.Presentation.Models;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation.Controllers.Locations;

[ApiController]
[Route("api/location")]
public class LocationController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateLocation(
        [FromBody] LocationRequest location,
        [FromServices] ICommandHandler<Guid, CreateLocationCommand> handler,
        CancellationToken cancellationToken)
    {
        var result =
            await handler.Handle(new CreateLocationCommand(location.Name, location.Address, location.TimeZone),
                cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToErrorResponse();
        }

        return Ok(Envelop.Ok(result.Value));
    }
}

public class LocationRequest
{
    public string Address { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string TimeZone { get; set; } = string.Empty;
}