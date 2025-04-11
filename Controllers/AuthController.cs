using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherAPI.Data;
using WeatherAPI.Models;
using WeatherAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace WeatherAPI.Controllers;

[Route("api/v1/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;
    private readonly AppDbContext _appContext;

    public AuthController(TokenService tokenService, AppDbContext appContext)
    {
        _tokenService = tokenService;
        _appContext = appContext;
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> LoginAsync(LoginUserDTO user)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { message = "Invalid proprieties." });

        var existingUser = await _appContext.Users
            .FirstOrDefaultAsync(u => u.Username == user.Username && u.Email == user.Email);

        if (existingUser == null || !TokenService.VerifyPassword(user.Password, existingUser.PasswordHash))
            return Unauthorized(new { message = "Invalid username or password." });

        var userToken = _tokenService.GenerateTokenUser(existingUser);

        existingUser.LastLogin = DateTime.UtcNow;
        await _appContext.SaveChangesAsync();

        return Ok(new { message = "Login successful.", userToken = userToken });
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> RegisterAsync(PostUserDTO user)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { message = "Invalid username or password." });

        var existingUser = await _appContext.Users
            .FirstOrDefaultAsync(u => u.Username == user.Username || u.Email == user.Email);

        if (existingUser != null)
            return Conflict(new { message = "The username or email is already registered." });

        var newUser = new UserModel
        {
            Username = user.Username,
            Email = user.Email,
            CreatedAt = DateTime.UtcNow,
            PasswordHash = TokenService.HashPassword(user.Password),
            Roles = ["user"]
        };

        await _appContext.Users.AddAsync(newUser);
        await _appContext.SaveChangesAsync();

        return Ok(new { message = "User registered successful.", id = newUser.Id });
    }

    [Authorize(Roles = "admin")]
    [HttpGet("users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IAsyncEnumerable<ReturnUserDTO>>> ListUsersAsync()
    {
        var users = await _appContext.Users
            .AsNoTracking()
            .Include(u => u.Devices)
            .ToListAsync();

        var returnUserDTOs = users.Select(u => new ReturnUserDTO
        {
            Username = u.Username,
            Email = u.Email,
            Devices = u.Devices.Select(d => new ReturnDeviceDTO
            {
                Id = d.Id,
                DeviceName = d.DeviceName,
                DeviceToken = d.DeviceToken,
                CreatedAt = d.CreatedAt
            }).ToList()
        }).ToList();

        return returnUserDTOs.Any() ? Ok(new { users = returnUserDTOs }) : NotFound(new { message = "Users not found." });
    }
}