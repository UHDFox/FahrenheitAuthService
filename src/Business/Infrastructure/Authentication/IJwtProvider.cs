using Domain.Entities.Users;

namespace Business.Infrastructure.Authentication;

public interface IJwtProvider
{
    public string GenerateToken(UserRecord user);
}