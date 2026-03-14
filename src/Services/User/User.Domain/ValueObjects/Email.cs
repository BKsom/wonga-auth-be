namespace User.Domain.ValueObjects;

public record Email
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty.", nameof(value));
        if (!value.Contains('@'))
            throw new ArgumentException("Email is invalid.", nameof(value));
        Value = value.Trim().ToLowerInvariant();
    }
}
