using MediatR;
using User.Application.DTOs;
using User.Domain.Entities;
using User.Domain.Repositories;

namespace User.Application.Commands.CreateUserProfile;

public class CreateUserProfileCommandHandler : IRequestHandler<CreateUserProfileCommand, UserProfileDto>
{
    private readonly IUserProfileRepository _repository;

    public CreateUserProfileCommandHandler(IUserProfileRepository repository) => _repository = repository;

    public async Task<UserProfileDto> Handle(CreateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = UserProfile.Create(request.Id, request.FirstName, request.LastName, request.Email);
        await _repository.AddAsync(profile, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return new UserProfileDto(profile.Id, profile.FirstName, profile.LastName, profile.Email);
    }
}
