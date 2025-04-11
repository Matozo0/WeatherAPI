using System.ComponentModel.DataAnnotations;

namespace WeatherAPI.Models;

public class PostDeviceDTO
{
    [Required]
    public string DeviceName { get; set; } 
}