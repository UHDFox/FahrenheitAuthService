namespace Business.Infrastructure.Exceptions;

public class LoginException : Exception
{
    public LoginException(string? message = default) : base(message)
    {
    }
}