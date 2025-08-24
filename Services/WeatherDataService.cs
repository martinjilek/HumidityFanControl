using System.Net.Http.Json;
using HumidityFanControl.Config;
using HumidityFanControl.Data.Models;
using Microsoft.Extensions.Options;

namespace HumidityFanControl.Services;

public interface IWeatherDataService
{
    Task<WeatherData?> GetWeatherData(double lat, double lon);
    double Humidity { get; }
    event Action<double>? GetHumidityOnWeatherDataReceived;
}

public class WeatherDataService : IWeatherDataService
{
    private readonly HttpClient _client;
    private readonly WeatherDataSettings _settings;
    private Timer? _timer;

    public WeatherData? WeatherData { get; private set; }
    public event Action<double>? GetHumidityOnWeatherDataReceived;

    public double Humidity
    {
        get
        {
            if (WeatherData?.Hourly?.Time == null || WeatherData.Hourly.Relative_humidity_2m == null)
                return 0.0;

            // Convert API times to DateTime
            var times = WeatherData.Hourly.Time
                .Select(t => DateTime.Parse(t)) // API gives ISO8601 strings
                .ToList();

            DateTime now = DateTime.Now;

            // Find index of closest time to "now"
            int closestIndex = times
                .Select((t, i) => new { Time = t, Index = i, Diff = Math.Abs((t - now).TotalMinutes) })
                .OrderBy(x => x.Diff)
                .First().Index;

            return WeatherData.Hourly.Relative_humidity_2m[closestIndex];
        }
    }

    public WeatherDataService(HttpClient client, IOptions<WeatherDataSettings> settings)
    {
        _client = client;
        _settings = settings.Value;
        if (!_settings.Enable) return;

        // Start the polling loop
        _timer = new Timer(async _ => await FetchWeatherData(), null, TimeSpan.Zero, TimeSpan.FromMinutes(30));
    }

    public async Task<WeatherData?> GetWeatherData(double lat, double lon)
    {
        return await _client.GetFromJsonAsync<WeatherData?>(
            $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&current_weather=true&hourly=relative_humidity_2m&timezone=Europe%2FPrague");
    }

    private async Task FetchWeatherData()
    {
        try
        {
            WeatherData = await GetWeatherData(_settings.Latitude, _settings.Longitude);
            GetHumidityOnWeatherDataReceived?.Invoke(Humidity);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching weather data: {ex.Message}");
        }
    }
}