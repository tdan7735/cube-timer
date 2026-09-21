using backend.Data;
using Microsoft.EntityFrameworkCore;
using backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Services
builder.Services.AddControllers();
builder.Services.AddScoped<StatisticsService>();
builder.Services.AddScoped<ScrambleService>();
builder.Services.AddScoped<AlgorithmService>();

// Add database support
var connectionString = builder.Configuration.GetValue<string>("Db:DefaultConnection");
if (connectionString == null) {
    Console.WriteLine("No connection string found");
    return;
}

Console.WriteLine("Connection string found");

builder.Services.AddDbContext<AppDbContext>(options => {
    options.UseNpgsql(connectionString);
});

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope()) {
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await UserSeeder.SeedAsync(context);
    await AlgorithmSeeder.SeedAsync(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Map controller endpoints
app.MapControllers();

Console.WriteLine("Starting server...");
app.Run();
