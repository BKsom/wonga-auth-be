using Auth.Application.DTOs;
using MediatR;

namespace Auth.Application.Commands.LoginUser;

public record LoginUserCommand(string Email, string Password) : IRequest<LoginResponseDto>;
