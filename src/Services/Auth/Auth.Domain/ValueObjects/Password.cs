namespace Auth.Domain.ValueObjects;

public record Password
{
    public string Value { get; }

    public Password(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Password cannot be empty.", nameof(value));
        if (value.Length < 6)
            throw new ArgumentException("Password must be at least 6 characters.", nameof(value));
        Value = value;
    }
}
