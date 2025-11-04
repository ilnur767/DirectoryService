using DirectoryService.Application.Validation;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;

namespace DirectoryService.Application.Commands.Departments.CreateDepartment;

public class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentCommandValidator()
    {
        RuleFor(c => c.Name).MustBeValueObject(DepartmentName.Create);
        RuleFor(c => c.Identifier).MustBeValueObject(Identifier.Create);
        RuleFor(c => c.LocationIds).MustContainUniqueElements();
    }
}