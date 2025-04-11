using System;
using Microsoft.EntityFrameworkCore;
using WeatherAPI.Models;

namespace WeatherAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<UserModel> Users { get; set; }
    public DbSet<DeviceModel> Devices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserModel>()
            .HasMany(u => u.Devices)
            .WithOne(d => d.User)
            .HasForeignKey(d => d.UserId);

        // modelBuilder.Entity<DeviceModel>()
        //     .HasMany(d => d.DeviceData)
        //     .WithOne(dd => dd.Device)
        //     .HasForeignKey(dd => dd.Guid);

        base.OnModelCreating(modelBuilder);
    }
}
