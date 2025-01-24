using Domain.Entities.Users;
using Web.Contracts.CommonResponses;
using Web.Contracts.User;

namespace FahrenheitAuthService.Client;

public interface IAuthClient
{
    Task<UserResponse> LoginAsync(LoginRequest request);

    Task<UserResponse> RegisterAsync(RegisterRequest request);

    Task<GetAllResponse<UserResponse>> GetListAsync(int? offset, int? limit);

    Task<UserResponse> GetByIdAsync(Guid id);

    Task<UserResponse> AddAsync(CreateUserRequest request);

    Task<UserResponse> UpdateAsync(UpdateUserRequest request);

    Task<bool> DeleteAsync(Guid id);
}