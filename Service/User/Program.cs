using User.Configuration.DbConfiguration.Extensions;
using User.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceBuilderExtensions(builder.Configuration);
builder.Configuration.AddJsonFile("appsettings.json");

builder.WebHost.ConfigureKestrel(op =>
{
    op.ListenAnyIP(4080, listenOp =>
    {
        listenOp.UseHttps();
        listenOp.Protocols =
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

