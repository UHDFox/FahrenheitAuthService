using AutoMapper;
using Business.Models.User;
using FahrenheitAuthService.Contracts.Contracts.CommonResponses;
using FahrenheitAuthService.Contracts.Contracts.User;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public UserController(IUserService userService, IMapper mapper, ILogger<UserController> logger)
    {
        _userService = userService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginRequest request)
    {
        var token = await _userService.LoginAsync(_mapper.Map<LoginModel>(request), HttpContext);

        if (string.IsNullOrEmpty(token)) return Unauthorized("Invalid credentials");

        return Ok(token);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegisterRequest request)
    {
        var result = await _userService.RegisterAsync(_mapper.Map<RegisterModel>(request));

        return Created($"{Request.Path}", _mapper.Map<UserResponse>(await _userService.GetByIdAsync(result)));
    }


    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAllResponse<UserRecord>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetListAsync(int? offset, int? limit)
    {
        _logger.LogInformation($"Received request for users: offset={offset}, limit={limit}");
        try
        {
            var result = await _userService.GetListAsync(offset.GetValueOrDefault(0), limit.GetValueOrDefault(5));
            _logger.LogInformation("Request processed successfully.");
            return Ok(new GetAllResponse<UserRecord>(_mapper.Map<IReadOnlyCollection<UserRecord>>(result),
                result.Count));
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in GetListAsync: {Message}", ex.Message);
            return StatusCode(500, "An error occurred while processing the request.");
        }
    }

    [HttpGet("id:guid")]
    [Authorize(Roles = "SuperAdmin, HighLevelAdmin, LowLevelAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserRecord))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        return Ok(_mapper.Map<UserResponse>(await _userService.GetByIdAsync(id)));
    }

    [HttpGet("email")]
    [Authorize(Roles = "SuperAdmin, HighLevelAdmin, LowLevelAdmin, User")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserRecord))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByEmailAsync(string email)
    {
        return Ok(await _userService.GetByEmailAsync(email));
    }


    [HttpPost]
    [Authorize(Roles = "SuperAdmin, HighLevelAdmin, LowLevelAdmin")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreatedResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddAsync(CreateUserRequest data)
    {
        var result = await _userService.AddAsync(_mapper.Map<UserModel>(data));
        return Created($"{Request.Path}", _mapper.Map<UserResponse>(await _userService.GetByIdAsync(result)));
    }

    [HttpPut]
    [Authorize(Roles = "SuperAdmin, HighLevelAdmin, LowLevelAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdatedResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(UpdateUserRequest data)
    {
        await _userService.UpdateAsync(_mapper.Map<UserModel>(data));
        return Ok(new UpdatedResponse(data.Id));
    }

    [HttpDelete]
    [Authorize(Roles = "SuperAdmin, HighLevelAdmin, LowLevelAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DeletedResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var result = await _userService.DeleteAsync(id);
        return Ok(new DeletedResponse(id, result));
    }
}