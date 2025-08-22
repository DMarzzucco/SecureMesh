using System;
using Microsoft.EntityFrameworkCore;
using UserManagementService.Context;

namespace UserManagementService.Configuration.PostgreSQL.Extensions;

public static class AutoMigrationDataBase
{
    public static void ApplyAutoMigrations(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }
}
