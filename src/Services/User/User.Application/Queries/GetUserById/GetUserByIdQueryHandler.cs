using MediatR;
using User.Application.DTOs;
using User.Domain.Repositories;

namespace User.Application.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserProfileDto?>
{
    private readonly IUserProfileRepository _repository;

    public GetUserByIdQueryHandler(IUserProfileRepository repository) => _repository = repository;

    public async Task<UserProfileDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var profile = await _repository.GetByIdAsync(request.UserId, cancellationToken);
        if (profile is null) return null;
        return new UserProfileDto(profile.Id, profile.FirstName, profile.LastName, profile.Email);
    }
}
