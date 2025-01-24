using Microsoft.AspNetCore.Http;

namespace Business.Models.User;

public interface IUserService
{
    public Task<string> LoginAsync(LoginModel model, HttpContext context);
    
    public Task<Guid> RegisterAsync(RegisterModel model);

    public Task<IReadOnlyCollection<UserModel>> GetListAsync(int offset, int limit);

    public Task<Guid> AddAsync(UserModel userModel);

    public Task<UserModel> GetByIdAsync(Guid id);
    
    public Task<UserModel> GetByEmailAsync(string email);

    public Task<UserModel> UpdateAsync(UserModel userModel);
    
    public Task<bool> DeleteAsync(Guid id);

    public Task<int> SaveChangesAsync();
}