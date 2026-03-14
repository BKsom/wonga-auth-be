namespace User.Application.DTOs;

public record UserProfileDto(Guid Id, string FirstName, string LastName, string Email);
