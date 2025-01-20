using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Domain.Entities.Users;
using Microsoft.IdentityModel.Tokens;

namespace Business.Infrastructure.Authentication;

public sealed class JwtProvider : IJwtProvider
{
    public string GenerateToken(UserRecord user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, user.Role.ToString()), new(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var jwt = new JwtSecurityToken(
            AuthOptions.Issuer,
            AuthOptions.Audience,
            claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}