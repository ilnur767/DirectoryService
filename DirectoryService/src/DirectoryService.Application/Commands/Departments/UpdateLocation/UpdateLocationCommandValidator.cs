using DirectoryService.Application.Validation;
using FluentValidation;

namespace DirectoryService.Application.Commands.Departments.UpdateLocation;

public class UpdateLocationCommandValidator : AbstractValidator<UpdateDepartmentLocationCommand>
{
    public UpdateLocationCommandValidator()
    {
        RuleFor(c => c.DepartmentId).NotNull();
        RuleFor(c => c.LocationIds).NotNull().NotEmpty();
        RuleFor(c => c.LocationIds).MustContainUniqueElements();
    }
}