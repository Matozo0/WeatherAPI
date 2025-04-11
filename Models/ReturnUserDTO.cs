using System.ComponentModel.DataAnnotations;

namespace WeatherAPI.Models;

public class ReturnUserDTO
{
    public string Username { get; set; }
    public string Email { get; set; }
    public List<ReturnDeviceDTO> Devices { get; set; }
}