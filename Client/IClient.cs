using Microsoft.AspNetCore.Mvc;
using Web.Contracts.User;

namespace Client;

public interface IClient
{
    public Task<object> LoginAsync(LoginRequest request);

    public Task<object> RegisterAsync(RegisterRequest request);

    public Task<object> GetListAsync(int? offset, int? limit);

    public Task<object> GetByIdAsync(Guid id);

    public Task<object> AddAsync(CreateUserRequest request);

    public Task<object> UpdateAsync(UpdateUserRequest request);

    public Task<object> DeleteAsync(Guid id);

}