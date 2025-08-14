using HumidityFanControl.Config;
using HumidityFanControl.Data;
using HumidityFanControl.Hardware;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace HumidityFanControl.Services;

public class FanControlService : BackgroundService
{
    private readonly RelayController _relay;
    private readonly ISensorService _sensor;
    private readonly FanControlSettings _settings;
    private readonly IWeatherDataService _weatherDataService;
    
    private bool _relayOn = false;
    private double _outsideHumidity = 0.0;

    public FanControlService(RelayController relay, ISensorService sensor, IOptions<FanControlSettings> settings, IWeatherDataService weatherDataService)
    {
        _relay = relay;
        _sensor = sensor;
        _settings = settings.Value;
        _weatherDataService = weatherDataService;

        _weatherDataService.GetHumidityOnWeatherDataReceived += (data) =>
        {
            _outsideHumidity = data;
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            double temp = _sensor.ReadTemperature();
            double hum = _sensor.ReadHumidity();
            
            _relayOn = hum > _settings.HumidityThreshold;
            Console.WriteLine($"🌡️ {temp:F2}°C | 💧 {hum:F2}% | Relay: {(_relayOn ? "ON" : "OFF")} | Outside Humidity: {_outsideHumidity:F2}%");
            _relay.SetRelay(_relayOn);

            await Task.Delay(_settings.ReadIntervalMs, stoppingToken);
        }
    }
}