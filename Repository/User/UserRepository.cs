using Domain;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Repository.User;

public sealed class UserRepository : Repository<UserRecord>, IUserRepository
{
    public UserRepository(FahrenheitAuthDbContext context) : base(context)
    {
    }

    public Task<UserRecord?> GetByEmailAsync(string email)
        => _context.Users.FirstOrDefaultAsync(e => e.Email == email);

    public void Update(UserRecord data)
    {
        //var user = _context.Users.FirstOrDefaultAsync(e => e.Email == model.Email)
        //    ?? throw new Exception($"User with email {model.Email} does not exist while trying to update with email {model.Email}");
        
        _context.Users.Update(data);
        _context.Entry(data).Property(x => x.PasswordHash).IsModified = true;
    }
}