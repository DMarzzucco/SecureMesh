using SecureMesh.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceBuilderExtensions(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseApplicationBuilderExtensions();
app.MapReverseProxy();
app.Run();
