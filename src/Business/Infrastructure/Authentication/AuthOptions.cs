using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Business.Infrastructure.Authentication;

public static class AuthOptions
{
    public const string Issuer = "FahrenheitAuthService";
    public const string Audience = "FahrenheitAuth";

    private const string Key =
        "testSecretKeyLoooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong"; // encryption key

    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
    }
}