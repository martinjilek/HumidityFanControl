using HumidityFanControl.Config;
using HumidityFanControl.Data;
using HumidityFanControl.Hardware;
using HumidityFanControl.Data.Models;
using HumidityFanControl.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true)
    .Build();

builder.Services.Configure<FanControlSettings>(configuration.GetSection("FanControl"));
builder.Services.Configure<SensorSettings>(configuration.GetSection("Sensor"));
builder.Services.Configure<WeatherDataSettings>(configuration.GetSection("WeatherDataSettings"));

builder.Services.AddSingleton<IWeatherDataService, WeatherDataService>();
builder.Services.AddSingleton<ISensorService, Htu21dSensorService>();
builder.Services.AddSingleton<RelayController>();

builder.Services
    .AddDbContext<HfcContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

// Register the log repository
builder.Services.AddScoped<ILogRepository, DataLogger>();

builder.Services.AddHostedService<FanControlService>();

builder.Services.AddHttpClient<WeatherDataService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HfcContext>();
    try
    {
        var pending = db.Database.GetPendingMigrations();
        if (pending.Any())
        {
            Console.WriteLine("Applying database migrations...");
            db.Database.Migrate();
            Console.WriteLine("Migrations applied successfully.");
        }
        else
        {
            Console.WriteLine("Database is already up to date.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error applying migrations: {ex.Message}");
    }
}

await app.RunAsync();
