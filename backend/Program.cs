using backend.Controllers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add controller support
builder.Services.AddControllers();

// Add database support
builder.Services.AddDbContext<SolveContext>(options => {
    options.UseNpgsql(builder.Configuration.GetValue<string>("Db:ConnectionString"));
});

var app = builder.Build();

var ConnectionString = app.Configuration.GetValue<string>("Db:ConnectionString");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Map controller endpoints
app.MapControllers();

Console.WriteLine("Starting server...");
Console.WriteLine("Connection String: " + ConnectionString);
app.Run();
