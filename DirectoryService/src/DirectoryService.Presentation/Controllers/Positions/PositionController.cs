using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Commands.Positions.CreatePosition;
using DirectoryService.Presentation.Models;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation.Controllers.Positions;

[ApiController]
[Route("api/position")]
public class PositionController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreatePosition(
        [FromBody] PositionRequest location,
        [FromServices] ICommandHandler<Guid, CreatePositionCommand> handler,
        CancellationToken cancellationToken)
    {
        var result =
            await handler.Handle(new CreatePositionCommand(location.Name, location.Description, location.DepartmentIds),
                cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToErrorResponse();
        }

        return Ok(Envelop.Ok(result.Value));
    }
}

public class PositionRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid[] DepartmentIds { get; set; } = [];
}