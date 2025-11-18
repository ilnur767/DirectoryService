using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Commands.Departments.CreateDepartment;
using DirectoryService.Application.Commands.Departments.MoveDepartment;
using DirectoryService.Application.Commands.Departments.UpdateLocation;
using DirectoryService.Presentation.Models;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation.Controllers.Departments;

[ApiController]
[Route("api/department")]
public class DepartmentsController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateDepartment(
        [FromBody] CreateDepartmentRequest request,
        [FromServices] ICommandHandler<Guid, CreateDepartmentCommand> handler,
        CancellationToken cancellationToken)
    {
        var result =
            await handler.Handle(
                new CreateDepartmentCommand(request.Name, request.Identifier, request.ParentId, request.LocationIds),
                cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToErrorResponse();
        }

        return Ok(Envelop.Ok(result.Value));
    }

    [HttpPut("{id:guid}/locations")]
    public async Task<IActionResult> UpdateLocation(
        [FromRoute] Guid id,
        [FromBody] Guid[] locationIds,
        [FromServices] ICommandHandler<UpdateDepartmentLocationCommand> handler,
        CancellationToken cancellationToken)
    {
        var result =
            await handler.Handle(
                new UpdateDepartmentLocationCommand(id, locationIds),
                cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToErrorResponse();
        }

        return NoContent();
    }

    [HttpPut("{id:guid}/parent")]
    public async Task<IActionResult> MoveDepartment(
        [FromRoute] Guid id,
        [FromBody] Guid? parentId,
        [FromServices] ICommandHandler<MoveDepartmentCommand> handler,
        CancellationToken cancellationToken)
    {
        var result =
            await handler.Handle(
                new MoveDepartmentCommand(id, parentId),
                cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToErrorResponse();
        }

        return NoContent();
    }
}

public class CreateDepartmentRequest
{
    public string Name { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public Guid[] LocationIds { get; set; } = [];
}