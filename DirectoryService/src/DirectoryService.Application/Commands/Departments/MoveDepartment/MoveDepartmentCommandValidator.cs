using FluentValidation;

namespace DirectoryService.Application.Commands.Departments.MoveDepartment;

public class MoveDepartmentCommandValidator : AbstractValidator<MoveDepartmentCommand>
{
    public MoveDepartmentCommandValidator() => RuleFor(m => m.DepartmentId).NotEmpty().NotNull();
}