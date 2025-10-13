using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Commands.AddLocation;
using DirectoryService.Presentation.Models;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation.Controllers;

[ApiController]
[Route("api/location")]
public class LocationController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddLocation(
        [FromBody] LocationRequest location,
        [FromServices] ICommandHandler<Guid, AddLocationCommand> handler,
        CancellationToken cancellationToken)
    {
        var result =
            await handler.Handle(new AddLocationCommand(location.Name, location.Address, location.TimeZone),
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