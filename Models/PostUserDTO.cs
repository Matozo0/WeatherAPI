using System.ComponentModel.DataAnnotations;

namespace WeatherAPI.Models;

public class PostUserDTO
{
    [Required]
    public string Username { get; set; }
    
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}