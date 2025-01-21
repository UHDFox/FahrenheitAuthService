using System.Net.Http.Json;
using Client.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Web.Contracts.User;

namespace Client.Implemetations;

public class Client : IClient
{
    private readonly HttpClient _httpClient;
    private readonly ClientOptions _options;

    public Client(HttpClient httpClient, IOptions<ClientOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _httpClient.BaseAddress = new Uri(_options.Uri);
    }
    
    public async Task<object> LoginAsync(LoginRequest request)
    {
        var response =  await _httpClient.PostAsJsonAsync("api/v1/User/login", request);

        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<string>()
                ?? throw new HttpRequestException("Error while logging in.");

            return data;
        }
        return new UnauthorizedResult();
    }

    public async Task<object> RegisterAsync(RegisterRequest request)
    {
        var response =  await _httpClient.PostAsJsonAsync("api/v1/User/register", request);

        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<string>()
                       ?? throw new HttpRequestException("Error while logging in.");

            return data;
        }
        return new UnauthorizedResult();
    }

    public async Task<object> GetListAsync(int? offset, int? limit)
    {
        var response =  await _httpClient.GetAsync($"api/v1/User?&offset={offset}limit?={limit}");

        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<string>()
                       ?? throw new HttpRequestException("Error while logging in.");

            return data;
        }
        return new UnauthorizedResult();
    }

    public async Task<object> GetByIdAsync(Guid id)
    {
        var response =  await _httpClient.GetAsync($"api/v1/User/id:{id}");

        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<string>()
                       ?? throw new HttpRequestException("Error while logging in.");

            return data;
        }
        return new UnauthorizedResult();
    }

    public async Task<object> AddAsync(CreateUserRequest request)
    {
        var response =  await _httpClient.PostAsJsonAsync("api/v1/User/", request);

        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<string>()
                       ?? throw new HttpRequestException("Error while logging in.");

            return data;
        }
        return new UnauthorizedResult();
    }

    public async Task<object> UpdateAsync(UpdateUserRequest request)
    {
        var response =  await _httpClient.PutAsJsonAsync("api/v1/User", request);

        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<string>()
                       ?? throw new HttpRequestException("Error while logging in.");

            return data;
        }
        return new UnauthorizedResult();
    }

    public async Task<object> DeleteAsync(Guid id)
    {
        var response =  await _httpClient.DeleteAsync($"api/v1/User?id={id}");

        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<string>()
                       ?? throw new HttpRequestException("Error while logging in.");

            return data;
        }
        return new UnauthorizedResult();
    }
}