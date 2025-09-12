using Gateway.Core.Extensions;
using Gateway.Routing.Extensions;
using MinimalEndpoints.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add modules services
builder.Services
    .AddGatewayCore(builder.Configuration)
    .AddGatewayRouting(builder.Configuration);

// Add other services
builder.Services.AddSwaggerGen();
builder.Services.AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();

// Add modules middlewares
app
    .UseGatewayCore()
    .UseGatewayRouting();

app.UseHttpsRedirection();

app.MapEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();


