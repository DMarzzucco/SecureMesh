using Microsoft.EntityFrameworkCore;
using User.Context.Config;
using User.Module.Model;

namespace User.Context;

public class AppDbContext:DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
    {
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.EnableSensitiveDataLogging();
    }
    public DbSet<UserModel> UserModel { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserModelConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}