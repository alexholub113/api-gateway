using Gateway.Core.Extensions;
using Gateway.Routing.Extensions;
using MinimalEndpoints.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add modules services
builder.Services
    .AddGatewayCore(builder.Configuration)
    .AddGatewayRouting(builder.Configuration);

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

// Add modules middlewares
app
    .UseGatewayCore()
    .UseGatewayRouting();

app.Run();


