using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using WeatherAPI.Data;
using WeatherAPI.Models;

namespace WeatherAPI.Services;

public static class DbInitializer
{
    public static void SeedAdminUser(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var adminExists = context.Users.Any(u => u.Username == "admin" && u.Email == "@admin.io");

        if (!adminExists)
        {
            var admin = new UserModel
            {
                Username = "admin",
                Email = "@admin.io",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"),
                Roles = new[] { "admin" },
                CreatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow
            };

            context.Users.Add(admin);
            context.SaveChanges();
        }
    }
}