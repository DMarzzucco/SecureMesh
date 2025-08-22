using System;
using Microsoft.EntityFrameworkCore;
using SessionsManagement.Context.Config;
using SessionsManagement.Module.Model;

namespace SessionsManagement.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.EnableSensitiveDataLogging();
    }
    public DbSet<SessionModel> SessionModel { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new SecurirtyModelConfig());
        base.OnModelCreating(modelBuilder);
    }
}


