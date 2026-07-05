namespace cico.Domain.ValueObjects;

public record EmployeeCode(string Value)
{
    public static implicit operator string(EmployeeCode code)
        => code.Value;

    public static EmployeeCode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                "Employee code cannot be empty.",
                nameof(value));

        return new EmployeeCode(value);
    }
}
