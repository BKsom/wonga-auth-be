using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using User.Application.Commands.CreateUserProfile;
using User.Domain.Repositories;
using User.Infrastructure.Data;
using User.Infrastructure.Repositories;

namespace User.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UserDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("UserDatabase")));

        services.AddScoped<IUserProfileRepository, UserProfileRepository>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateUserProfileCommand).Assembly));
        services.AddValidatorsFromAssembly(typeof(CreateUserProfileCommand).Assembly);

        return services;
    }
}
