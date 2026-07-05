namespace cico.Domain.ValueObjects;

public record PhoneNumber(string Value)
{
    public static implicit operator string(PhoneNumber phone)
        => phone.Value;

    public static PhoneNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                "Phone number cannot be empty.",
                nameof(value));

        return new PhoneNumber(value);
    }
}
