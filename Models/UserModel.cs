using System.ComponentModel.DataAnnotations;

namespace WeatherAPI.Models;

public class UserModel
{   
    [Key]
    public int Id { get; set; }

    [Required]
    public string Username { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }
    
    [Required]
    public string[] Roles { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime LastLogin { get; set; }

    public ICollection<DeviceModel> Devices { get; set; } = new List<DeviceModel>();
} 