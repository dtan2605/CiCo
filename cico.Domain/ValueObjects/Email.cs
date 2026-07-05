namespace cico.Domain.ValueObjects;

public record Email(string Value)
{
    public static implicit operator string(Email email)
        => email.Value;

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                "Email cannot be empty.",
                nameof(value));

        if (!value.Contains('@'))
            throw new ArgumentException(
                "Email must contain '@'.",
                nameof(value));

        return new Email(value);
    }
}
