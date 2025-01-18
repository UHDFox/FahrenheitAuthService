namespace Business.Infrastructure.Exceptions;

public sealed class PaginationQueryException : Exception
{
    public PaginationQueryException(string? message = default) : base(message)
    {
    }
}