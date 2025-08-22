using System;
using Microsoft.EntityFrameworkCore;
using UserManagementService.Context.Config;
using UserManagementService.Modules.Models;

namespace UserManagementService.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.EnableSensitiveDataLogging();
    }

    public DbSet<ManagementUserModel> ManagementUser { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ManagementUserModelConfig());
        
        base.OnModelCreating(modelBuilder);
    }
}
