using backend.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add controller support
builder.Services.AddControllers();

// Add database support
var connectionString = builder.Configuration.GetValue<string>("Db:DefaultConnection");
if (connectionString == null) {
    Console.WriteLine("No connection string found");
    return;
}

Console.WriteLine("Connection string found");

builder.Services.AddDbContext<SolveContext>(options => {
    options.UseNpgsql(connectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Map controller endpoints
app.MapControllers();

Console.WriteLine("Starting server...");
app.Run();
