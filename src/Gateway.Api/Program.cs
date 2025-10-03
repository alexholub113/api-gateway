using Gateway.Core.Extensions;
using MinimalEndpoints.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGateway(builder.Configuration);

// Add API Explorer services (required for Swagger)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapEndpoints();

app.UseGateway();

app.Run();


