using Microsoft.Extensions.DependencyInjection;
using Repository.User;

namespace Repository.Infrastructure;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IUserRepository, UserRepository>();

        return services;
    }
}