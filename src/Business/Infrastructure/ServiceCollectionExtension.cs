using Business.Infrastructure.Authentication;
using Business.Models.User;
using Microsoft.Extensions.DependencyInjection;

namespace Business.Infrastructure;

public static class ServiceCollectionExtension
{
    public static void AddBusinessServices(this IServiceCollection services)
    {
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IJwtProvider, JwtProvider>();
        services.AddTransient<IPasswordProvider, PasswordProvider>();

        services.AddHttpContextAccessor();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }
}