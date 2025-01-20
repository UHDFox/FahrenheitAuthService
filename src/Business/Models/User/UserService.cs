using AutoMapper;
using Business.Infrastructure.Authentication;
using Business.Infrastructure.Exceptions;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Repository.User;

namespace Business.Models.User;

internal sealed class UserService : IUserService
{
    private readonly IJwtProvider _jwtProvider;
    private readonly ILogger<UserService> _logger;
    private readonly IMapper _mapper;
    private readonly IPasswordProvider _passwordProvider;
    private readonly IUserRepository _repository;

    public UserService(
        IMapper mapper,
        IUserRepository repository,
        IJwtProvider jwtProvider,
        IPasswordProvider passwordProvider,
        ILogger<UserService> logger)
    {
        _mapper = mapper;
        _repository = repository;
        _jwtProvider = jwtProvider;
        _passwordProvider = passwordProvider;
        _logger = logger;
    }

    public async Task<Guid> AddAsync(UserModel userModel)
    {
        var entity = _mapper.Map<UserRecord>(userModel);

        entity.PasswordHash = _passwordProvider.Generate(userModel.Password);

        var result = await _repository.AddAsync(_mapper.Map<UserRecord>(entity));

        _logger.LogInformation($"Created new user with id {result}");

        return result;
    }

    public async Task<string> LoginAsync(LoginModel model, HttpContext context)
    {
        var user = await _repository.GetByEmailAsync(model.Email);

        if (user == null)
        {
            _logger.LogError($"User with email {model.Email} does not exist");
            throw new LoginException("can't login - user with stated mail not found");
        }

        if (!_passwordProvider.Verify(model.Password, user.PasswordHash))
        {
            _logger.LogError($"User with email {model.Email} does not match password: {model.Password}");
            throw new LoginException("Password mismatch");
        }

        var token = _jwtProvider.GenerateToken(user);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(20)
        };

        context.Response.Cookies.Append("some-cookie", token, cookieOptions);

        return token;
    }

    public async Task<Guid> RegisterAsync(RegisterModel model)
    {
        var result = await AddAsync(_mapper.Map<UserModel>(model));

        _logger.LogInformation($"Registered new user with id {result}");

        return result;
    }

    public async Task<IReadOnlyCollection<UserModel>> GetListAsync(int offset, int limit)
    {
        return _mapper.Map<IReadOnlyCollection<UserModel>>(await _repository.GetAllAsync(offset, limit));
    }

    public async Task<UserModel> GetByIdAsync(Guid id)
    {
        var record = await _repository.GetByIdAsync(id);
        if (record == null)
        {
            _logger.LogError($"Record with id: {id} not found in {nameof(UserModel)}");
            throw new NotFoundException($"Record not found in {nameof(UserModel)}");
        }

        _logger.LogInformation($"Retrieved record with id: {id} in {nameof(UserModel)}");

        return _mapper.Map<UserModel>(record);
    }

    public async Task<UserModel> UpdateAsync(UserModel userModel)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(userModel.Id);

            if (entity == null)
            {
                _logger.LogError($"Record with id: {userModel.Id} not found in {nameof(UserModel)} while updating");
                throw new NotFoundException($"Record with entity{userModel.Id} not found in {nameof(UserModel)}");
            }

            _mapper.Map(userModel, entity);

            entity.PasswordHash = _passwordProvider.Generate(userModel.Password);

            _repository.Update(entity);

            await _repository.SaveChangesAsync();

            _logger.LogInformation($"Successfully updated record with id: {userModel.Id} in {nameof(UserModel)}");

            return userModel;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                $"An error occured while updating entity with id: {userModel.Id}, \n Message: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            await GetByIdAsync(id);

            var result = await _repository.DeleteAsync(id);

            _logger.LogInformation(result
                ? $"Deleted record with Id: {id} in {nameof(UserModel)}"
                : $"Could not delete record with id: {id} in {nameof(UserModel)}");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while deleting record with Id: {id}, Message: {ex.Message}");
            return false;
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _repository.SaveChangesAsync();
    }
}