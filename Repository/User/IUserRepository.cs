using Domain.Entities.Users;

namespace Repository.User;

public interface IUserRepository : IRepository<UserRecord>
{
    public Task<UserRecord?> GetByEmailAsync(string email);
}