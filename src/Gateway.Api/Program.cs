using Gateway.Core.Extensions;
using Gateway.LoadBalancing.Extensions;
using Gateway.Proxy.Extensions;
using Gateway.ServiceRouting.Extensions;
using MinimalEndpoints.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add modules services
builder.Services
    .AddGatewayCore(builder.Configuration)
    .AddGatewayServiceRouting(builder.Configuration)
    .AddGatewayProxy(builder.Configuration)
    .AddLoadBalancing();

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


