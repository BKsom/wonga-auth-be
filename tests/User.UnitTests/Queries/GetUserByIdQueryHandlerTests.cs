using FluentAssertions;
using Moq;
using User.Application.DTOs;
using User.Application.Queries.GetUserById;
using User.Domain.Entities;
using User.Domain.Repositories;
using Xunit;

namespace User.UnitTests.Queries;

public class GetUserByIdQueryHandlerTests
{
    private readonly Mock<IUserProfileRepository> _repositoryMock;
    private readonly GetUserByIdQueryHandler _handler;

    public GetUserByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IUserProfileRepository>();
        _handler = new GetUserByIdQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingUser_ReturnsUserProfileDto()
    {
        var id = Guid.NewGuid();
        var profile = UserProfile.Create(id, "Jane", "Doe", "jane@example.com");
        _repositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(profile);

        var result = await _handler.Handle(new GetUserByIdQuery(id), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.Email.Should().Be("jane@example.com");
    }

    [Fact]
    public async Task Handle_NonExistingUser_ReturnsNull()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((UserProfile?)null);

        var result = await _handler.Handle(new GetUserByIdQuery(id), CancellationToken.None);

        result.Should().BeNull();
    }
}
