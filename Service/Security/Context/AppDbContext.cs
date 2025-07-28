using System;
using Microsoft.EntityFrameworkCore;
using Security.Context.Config;
using Security.Module.Model;

namespace Security.Context;

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


