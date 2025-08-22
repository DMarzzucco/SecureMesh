using System;
using IdentifyService.Context;
using Microsoft.EntityFrameworkCore;

namespace IdentifyService.Configuration.PostgreSQL.Helper;

public static class AutoMigrations
{
    public static void ApplyAutoMigration(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }
}