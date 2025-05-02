using Microsoft.Extensions.Options;
using SecureMesh.Extensions;
using Yarp.ReverseProxy.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");

builder.WebHost.ConfigureKestrel(op =>
{
    op.ListenAnyIP(8888, listen =>
    {
        listen.UseHttps();
        listen.Protocols=
            Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
    });
});


builder.Services.AddServiceBuilderExtensions(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(op =>
    {
        var config = app.Services.GetRequiredService<IOptionsMonitor<ReverseProxyDocumentFilterConfig>>().CurrentValue;
        foreach (var cluster in config.Clusters)
        {
            op.SwaggerEndpoint($"/swagger/{cluster.Key}/swagger.json", cluster.Key);
        }
    });
}

app.UseApplicationBuilderExtensions();
app.MapControllers();
app.MapReverseProxy();
app.Run();
