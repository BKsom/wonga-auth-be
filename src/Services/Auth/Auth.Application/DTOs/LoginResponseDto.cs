namespace Auth.Application.DTOs;

public record LoginResponseDto(string Token, UserDto User);
