using Domain.Entities.Users;

namespace Repository.User;

public interface IUserRepository
{
    Task<IReadOnlyCollection<UserRecord>> GetAllAsync(int offset, int limit);

    Task<UserRecord> GetByEmailAsync(string email);

    Task<int> GetTotalAmountAsync();

    Task<UserRecord?> GetByIdAsync(Guid id);

    Task<Guid> AddAsync(UserRecord data);

    void Update(UserRecord data);

    Task<bool> DeleteAsync(Guid id);

    Task<int> SaveChangesAsync();
}