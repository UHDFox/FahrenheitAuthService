using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Domain;

public class FahrenheitAuthDbContext : DbContext
{
    public FahrenheitAuthDbContext(DbContextOptions<FahrenheitAuthDbContext> options) : base(options)
    {
    }
    
    public DbSet<UserRecord> Users { get; set; }
}