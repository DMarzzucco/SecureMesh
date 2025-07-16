using Security.Configurations.Connections.Extensions;
using Security.Extensions;
using Security.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.AddServicesBuilderExtensions(builder.Configuration);

builder.WebHost.ConfigureKestrel(o =>
{
    o.ListenAnyIP(6070, listen =>
    {
        listen.UseHttps();
        listen.Protocols =
            Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
app.ApplyAutoMigrations();
app.UseApplicationBuilderExtensions();

app.Run();
