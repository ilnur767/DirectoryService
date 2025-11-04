using DirectoryService.Application.Validation;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;

namespace DirectoryService.Application.Commands.Locations.CreateLocation;

public class CreateLocationValidator : AbstractValidator<CreateLocationCommand>
{
    public CreateLocationValidator()
    {
        RuleFor(x => x.Name).MustBeValueObject(LocationName.Create);
        RuleFor(x => x.Address).MustBeValueObject(Address.Create);
        RuleFor(x => x.TimeZone).MustBeValueObject(Timezone.Create);
    }
}