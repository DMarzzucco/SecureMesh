using Microsoft.EntityFrameworkCore;
using User.Context;

namespace User.Configuration.DbConfiguration.Extensions;

public static class AutoMigrationDataBase
{
    public static void ApplyAutoMigrations(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }
}