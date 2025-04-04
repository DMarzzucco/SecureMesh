using Security.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceBuilderExtensions(builder.Configuration);

builder.WebHost.ConfigureKestrel(op =>
{
    op.ListenAnyIP(5090, listen =>
    {
        listen.UseHttps();
        listen.Protocols =
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

app.UseApplicationBuilderExtensions();
app.MapControllers();
app.Run();
