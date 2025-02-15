using System.Net.Http.Json;
using Contracts.Contracts.CommonResponses;
using Contracts.Contracts.User;
using FahrenheitAuthService.Client.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FahrenheitAuthService.Client.Implemetations;

public class AuthClient : IAuthClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthClient> _logger;

    public AuthClient(HttpClient httpClient, IOptions<AuthServiceOptions> options, ILogger<AuthClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        var serviceOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _httpClient.BaseAddress = new Uri(serviceOptions.Uri);
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<UserResponse> LoginAsync(LoginRequest request)
    {
        return await SendAsync<UserResponse>("api/v1/User/login", HttpMethod.Post, request, "Login");
    }

    public async Task<UserResponse> RegisterAsync(RegisterRequest request)
    {
        return await SendAsync<UserResponse>("api/v1/User/register", HttpMethod.Post, request, "Register");
    }

    public async Task<GetAllResponse<UserResponse>> GetListAsync(int? offset, int? limit)
    {
        var query = $"api/v1/User?offset={offset}&limit={limit}";
        return await SendAsync<GetAllResponse<UserResponse>>(query, HttpMethod.Get, null, "GetList");
    }

    public async Task<UserResponse> GetByIdAsync(Guid id)
    {
        var query = $"api/v1/User/{id}";
        return await SendAsync<UserResponse>(query, HttpMethod.Get, null, "GetById");
    }

    public async Task<UserResponse> GetByEmailAsync(string email)
    {
        var query = $"api/v1/User/email/{email}";
        return await SendAsync<UserResponse>(query, HttpMethod.Get, null, "GetByEmail");
    }

    public async Task<UserResponse> AddAsync(CreateUserRequest request)
    {
        return await SendAsync<UserResponse>("api/v1/User", HttpMethod.Post, request, "AddUser");
    }

    public async Task<UserResponse> UpdateAsync(UpdateUserRequest request)
    {
        return await SendAsync<UserResponse>("api/v1/User", HttpMethod.Put, request, "UpdateUser");
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var query = $"api/v1/User/{id}";
        return await SendAsync<bool>(query, HttpMethod.Delete, null, "DeleteUser");
    }

    private async Task<T> SendAsync<T>(string endpoint, HttpMethod method, object? payload, string operation)
    {
        try
        {
            _logger.LogInformation("{Operation} request to {Url}", operation, _httpClient.BaseAddress + endpoint);

            var request = new HttpRequestMessage(method, endpoint)
            {
                Content = payload != null ? JsonContent.Create(payload) : null
            };

            var response = await _httpClient.SendAsync(request);

            _logger.LogInformation("{Operation} response status: {StatusCode}", operation, response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<T>();
                if (data == null)
                {
                    throw new HttpRequestException($"Failed to deserialize response for {operation}");
                }

                _logger.LogInformation("{Operation} succeeded", operation);
                return data;
            }

            _logger.LogWarning("{Operation} failed with status code {StatusCode}", operation, response.StatusCode);
            throw new HttpRequestException($"Request failed with status code: {response.StatusCode}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "{Operation} failed due to HTTP error", operation);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Operation} failed due to unexpected error", operation);
            throw;
        }
    }
}
