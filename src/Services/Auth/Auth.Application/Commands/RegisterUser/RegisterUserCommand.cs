using Auth.Application.DTOs;
using MediatR;

namespace Auth.Application.Commands.RegisterUser;

public record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password
) : IRequest<UserDto>;
