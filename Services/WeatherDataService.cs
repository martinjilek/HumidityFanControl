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
            double result = 0.0;
            if (WeatherData?.Hourly?.Time != null)
            {
                string formattedTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:00");
                int index = WeatherData.Hourly.Time.IndexOf(formattedTime);
                if (index >= 0 && index < WeatherData.Hourly.Relative_humidity_2m.Count)
                {
                    result = WeatherData.Hourly.Relative_humidity_2m[index];
                }
            }
            return result;
        }
    }

    public WeatherDataService(HttpClient client, IOptions<WeatherDataSettings> settings)
    {
        _client = client;
        _settings = settings.Value;

        // Start the polling loop
        _timer = new Timer(async _ => await FetchWeatherData(), null, TimeSpan.Zero, TimeSpan.FromMinutes(30));
    }

    public async Task<WeatherData?> GetWeatherData(double lat, double lon)
    {
        return await _client.GetFromJsonAsync<WeatherData?>(
            $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&current_weather=true&hourly=relative_humidity_2m");
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