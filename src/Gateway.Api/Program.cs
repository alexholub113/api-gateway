using MinimalEndpoints.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
builder.Services.AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();


