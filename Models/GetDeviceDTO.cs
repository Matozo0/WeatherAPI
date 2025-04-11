using System.ComponentModel.DataAnnotations;

namespace WeatherAPI.Models;

public class GetDeviceDTO
{
    [Required]
    public Guid Id { get; set; } 
}