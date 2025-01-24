using Domain;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Repository.User;

public sealed class UserRepository : IUserRepository
{
    protected FahrenheitAuthDbContext _context;

    public UserRepository(FahrenheitAuthDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<UserRecord>> GetAllAsync(int offset, int limit)
    {
        return await _context.Set<UserRecord>().Skip(offset).Take(limit).ToListAsync();
    }

    public async Task<UserRecord> GetByEmailAsync(string email)
    {
        return await _context.Set<UserRecord>().FirstOrDefaultAsync(e => e.Email == email)
               ?? throw new Exception("Email doesn't exist");
    }

    public async Task<int> GetTotalAmountAsync()
    {
        return await _context.Set<UserRecord>().CountAsync();
    }

    public async Task<UserRecord?> GetByIdAsync(Guid id)
    {
        return await _context.Set<UserRecord>().AsNoTracking().FirstOrDefaultAsync(record => record.Id == id);
    }

    public async Task<Guid> AddAsync(UserRecord data)
    {
        await _context.Set<UserRecord>().AddAsync(data);
        await SaveChangesAsync();
        return data.Id;
    }

    public void Update(UserRecord data)
    {
        _context.Set<UserRecord>().Update(data);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        _context.Set<UserRecord>().Remove((await GetByIdAsync(id))!);
        return await SaveChangesAsync() > 0;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}