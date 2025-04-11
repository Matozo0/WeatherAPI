using System.ComponentModel.DataAnnotations;

namespace WeatherAPI.Models;

public class UserDTO
{
    [Required]
    public string Username { get; set; }
    
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    public string[] Roles { get; set; } = [];
}