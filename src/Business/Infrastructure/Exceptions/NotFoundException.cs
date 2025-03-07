namespace Business.Infrastructure.Exceptions;

public sealed class NotFoundException : Exception
{
    public NotFoundException(string? message = default) : base(message)
    {
    }
}