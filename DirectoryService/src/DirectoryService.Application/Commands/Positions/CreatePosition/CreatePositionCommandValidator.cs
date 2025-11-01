using DirectoryService.Application.Validation;
using FluentValidation;

namespace DirectoryService.Application.Commands.Positions.CreatePosition;

public class CreatePositionCommandValidator : AbstractValidator<CreatePositionCommand>
{
    public CreatePositionCommandValidator() => RuleFor(p => p.DepartmentIds).MustContainUniqueElements();
}