using SecureMesh.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://*:8888");
builder.Services.AddServiceBuilderExtensions(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseApplicationBuilderExtensions();
app.MapControllers();
app.MapReverseProxy();
app.Run();
