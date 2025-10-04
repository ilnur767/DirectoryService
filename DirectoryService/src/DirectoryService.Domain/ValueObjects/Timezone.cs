using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared.Errors;
using static DirectoryService.Domain.Constants.LocationConstants;

namespace DirectoryService.Domain.ValueObjects;

public record Timezone
{
    private const string PATTERN =
        "^[A-Za-z]+(?:[_-][A-Za-z0-9]+)*/[A-Za-z0-9]+(?:[_-][A-Za-z0-9]+)*(?:/[A-Za-z0-9]+(?:[_-][A-Za-z0-9]+)*)*$";

    private Timezone(string value) => Value = value;
    private Timezone() { }
    public string Value { get; }

    public Result<Timezone, Error> Create(string name)
    {
        if (name.Length < TIMEZONE_MIN_LENGTH || name.Length > TIMEZONE_MAX_LENGTH)
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