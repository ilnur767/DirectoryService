using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Commands.Departments.CreateDepartment;
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
}

public class CreateDepartmentRequest
{
    public string Name { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public Guid[] LocationIds { get; set; } = [];
}