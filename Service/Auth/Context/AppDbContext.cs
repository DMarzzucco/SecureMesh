using System;
using Auth.Context.Config;
using Auth.Module.Model;
using Microsoft.EntityFrameworkCore;

namespace Auth.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.EnableSensitiveDataLogging();
    }

    public DbSet<AuthModel> AuthModel { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AuthModelConfig());
        base.OnModelCreating(modelBuilder);
    }
}
