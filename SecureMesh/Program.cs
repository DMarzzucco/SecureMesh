using Microsoft.Extensions.Options;
using SecureMesh.Extensions;
using Yarp.ReverseProxy.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://*:8888");
builder.Services.AddServiceBuilderExtensions(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI( op => {
        var config  = app.Services.GetRequiredService<IOptionsMonitor<ReverseProxyDocumentFilterConfig>>().CurrentValue;
        foreach ( var cluster in config.Clusters)
        {
            op.SwaggerEndpoint($"/swagger/{cluster.Key}/swagger.json", cluster.Key);
        }
    });
}

app.UseApplicationBuilderExtensions();
app.MapControllers();
app.MapReverseProxy();
app.Run();
