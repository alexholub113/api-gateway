using Gateway.Common.Extensions;
using Gateway.Core.Extensions;
using Gateway.LoadBalancing.Extensions;
using Gateway.Proxy.Extensions;
using Gateway.RateLimiting.Extensions;
using MinimalEndpoints.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add modules services
builder.Services
    .AddCommonServices()
    .AddGatewayCore()
    .AddGatewayProxy(builder.Configuration)
    .AddLoadBalancing()
    .AddRateLimiting();

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

app.Run();


