using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherAPI.Data;
using WeatherAPI.Models;
using WeatherAPI.Services;

namespace WeatherAPI.Controllers;

[Route("api/v1/device")]
[ApiController]
public class DeviceController : ControllerBase
{
    private readonly TokenService _tokenService;
    private readonly AppDbContext _appContext;

    public DeviceController(TokenService tokenService, AppDbContext appContext)
    {
        _tokenService = tokenService;
        _appContext = appContext;
    }

    [Authorize]
    [HttpPost("device")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateDeviceAsync(PostDeviceDTO device)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { message = "Invalid model." });

        var userId = User.FindFirstValue("UserId");
        if (string.IsNullOrEmpty(userId))
            return BadRequest(new { message = "User ID not found." });

        var deviceNameExist = await _appContext.Devices
            .AnyAsync(u => u.DeviceName == device.DeviceName);
        if (deviceNameExist)
            return BadRequest(new { message = "DeviceName already exist on your account."});

        var newId = Guid.NewGuid().ToString();

        var deviceToken = _tokenService.GenerateTokenDevice(new TokenDeviceDTO
        {
            Id = newId,
            UserId = int.Parse(userId),
            DeviceName = device.DeviceName
        });

        var user = _appContext.Users
            .Where(u => u.Id == int.Parse(userId));

        var newDevice = new DeviceModel
        {
            Id = newId,
            UserId = int.Parse(userId),
            DeviceToken = deviceToken,
            DeviceName = device.DeviceName,
            CreatedAt = DateTime.UtcNow
        };

        await _appContext.Devices.AddAsync(newDevice);
        await _appContext.SaveChangesAsync();

        var returnDeviceDto = new ReturnDeviceDTO
        {
            Id = newDevice.Id,
            DeviceName = newDevice.DeviceName,
            DeviceToken = newDevice.DeviceToken,
            CreatedAt = newDevice.CreatedAt
        };

        return Created("", new
        {
            message = "Device registered successfully.",
            device = returnDeviceDto            
        });
    }

    [Authorize]
    [HttpGet("devices")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ListDevicesAsync()
    {
        var devices = await _appContext.Devices
            .Where(d => d.UserId == Int32.Parse(User.FindFirstValue("UserId")))
            .AsNoTracking()
            .ToListAsync();

        var returnDeviceDtos = devices.Select(d => new ReturnDeviceDTO
        {
            Id = d.Id,
            DeviceName = d.DeviceName,
            DeviceToken = d.DeviceToken,
            CreatedAt = d.CreatedAt
        }).ToList();

        return returnDeviceDtos.Any() ? Ok(new { devices = returnDeviceDtos }) : NotFound(new { message = "Devices not found." });
    }

    // [Authorize]
    // [HttpGet("device/{id?}")]
    // [ProducesResponseType(StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status404NotFound)]
    // public async Task<ActionResult> GetDeviceAsync(id)
    // {   
    //     var device = await _appContext.Devices
    //         .FirstOrDefaultAsync(d => d.Id == getDeviceDTO.Id && d.UserId == Int32.Parse(User.FindFirstValue("UserId")));

    //     if (device == null)
    //         return NotFound(new { message = "Device not found."} );

    //     var returnDeviceDtos = new ReturnDeviceDTO
    //     {
    //         Id = device.Id,
    //         DeviceName = device.DeviceName,
    //         DeviceToken = device.DeviceToken,
    //         CreatedAt = device.CreatedAt
    //     }; 

    //     return Ok(new { device = returnDeviceDtos });
    // }

    [Authorize]
    [HttpDelete("device")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteDeviceAsync(DeleteDeviceDTO device)
    {
        var dbDevice = await _appContext.Devices
            .FirstOrDefaultAsync(d => d.Id == device.Id && d.UserId == Int32.Parse(User.FindFirstValue("UserId")));

        if (dbDevice == null)
            return NotFound(new { message = "Device not found." });

        _appContext.Devices.Remove(dbDevice);
        await _appContext.SaveChangesAsync();
        
        return Ok();
    }
}