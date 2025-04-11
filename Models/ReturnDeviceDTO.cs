namespace WeatherAPI.Models;

public class ReturnDeviceDTO
{
    public string Id { get; set; }
    public string DeviceName { get; set; } 
    public string DeviceToken { get; set; }
    public DateTime CreatedAt { get; set; }
}