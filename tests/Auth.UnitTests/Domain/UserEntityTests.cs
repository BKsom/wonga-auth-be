using Auth.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Auth.UnitTests.Domain;

public class UserEntityTests
{
    [Fact]
    public void Create_ValidData_CreatesUser()
    {
        var user = User.Create("John", "Doe", "john@example.com", "hashedpw");
        user.Should().NotBeNull();
        user.FirstName.Should().Be("John");
        user.LastName.Should().Be("Doe");
        user.Email.Should().Be("john@example.com");
        user.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_EmptyFirstName_ThrowsArgumentException()
    {
        var act = () => User.Create("", "Doe", "john@example.com", "hash");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_EmptyEmail_ThrowsArgumentException()
    {
        var act = () => User.Create("John", "Doe", "", "hash");
        act.Should().Throw<ArgumentException>();
    }
}
