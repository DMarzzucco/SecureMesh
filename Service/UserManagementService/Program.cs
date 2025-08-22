using UserManagementService.Configuration.PostgreSQL.Extensions;
using UserManagementService.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServicesBuilderExtensions(builder.Configuration);

builder.WebHost.ConfigureKestrel(o =>
{
    o.ListenAnyIP(7080, lp =>
    {
        lp.UseHttps();
        lp.Protocols =
            Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ApplyAutoMigrations();
app.UseApplicationBuilderExtensions();
app.MapControllers();
app.Run();