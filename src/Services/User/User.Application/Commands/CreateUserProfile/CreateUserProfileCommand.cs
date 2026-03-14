using MediatR;
using User.Application.DTOs;

namespace User.Application.Commands.CreateUserProfile;

public record CreateUserProfileCommand(Guid Id, string FirstName, string LastName, string Email) : IRequest<UserProfileDto>;
