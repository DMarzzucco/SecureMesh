using System;
using Microsoft.EntityFrameworkCore;
using SessionsManagement.Context;

namespace SessionsManagement.Configurations.Connections.Extensions;

public static class AutoMigrations
{
    public static void ApplyAutoMigrations(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }
}
