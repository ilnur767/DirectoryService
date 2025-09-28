using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared.Errors;

namespace DirectoryService.Domain.ValueObjects;

public record Timezone
{
    private const string PATTERN =
        "^[A-Za-z]+(?:[_-][A-Za-z0-9]+)*/[A-Za-z0-9]+(?:[_-][A-Za-z0-9]+)*(?:/[A-Za-z0-9]+(?:[_-][A-Za-z0-9]+)*)*$";

    private const int MIN_LENGTH = 3;
    private const int MAX_LENGTH = 120;

    private Timezone(string name) => Value = name;

    public string Value { get; }

    public Result<Timezone, Error> Create(string name)
    {
        if (name.Length < MIN_LENGTH || name.Length > MAX_LENGTH)
        {
            return Errors.General.ValueIsInvalid(nameof(Timezone));
        }

        if (Regex.IsMatch(name, PATTERN))
        {
            return Errors.General.ValueIsInvalid(nameof(Timezone));
        }

        return new Timezone(name);
    }
}