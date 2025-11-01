using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared.Errors;
using FluentValidation;

namespace DirectoryService.Application.Validation;

public static class CustomValidators
{
    public static IRuleBuilderOptionsConditions<T, TElement> MustBeValueObject<T, TElement, TValueObject>(
        this IRuleBuilder<T, TElement> ruleBuilder, Func<TElement, Result<TValueObject, Error>> factoryMethod) =>
        ruleBuilder.Custom((value, context) =>
        {
            var result = factoryMethod(value);
            if (result.IsSuccess)
            {
                return;
            }

            context.AddFailure(result.Error.Serialize());
        });

    public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule,
        Error error) => rule.WithMessage(error.Message);

    public static IRuleBuilderOptionsConditions<T, IEnumerable<TElement>> MustContainUniqueElements<T, TElement>(
        this IRuleBuilder<T, IEnumerable<TElement>> ruleBuilder) =>
        ruleBuilder.Custom((value, context) =>
        {
            var duplicateIds = value.GroupBy(v => v)
                .Where(d => d.Count() > 1)
                .Select(d => d.Key)
                .ToArray();

            if (duplicateIds.Length < 1)
            {
                return;
            }

            context.AddFailure(Errors.General.DuplicatesFound(context.PropertyPath).Serialize());
        });
}