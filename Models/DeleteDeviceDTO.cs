using System.ComponentModel.DataAnnotations;

namespace WeatherAPI.Models;

public class DeleteDeviceDTO
{
    [Required]
    public string Id { get; set; } 
}