using HumidityFanControl.Data;
using HumidityFanControl.Hardware;

namespace HumidityFanControl.Services;

public class FanControlService
{
    private readonly ISensorReader _sensor;
    private readonly RelayController _relay;
    private readonly DataLogger _logger;
    private readonly PeriodicTimer _timer;
    private readonly double _humidityThreshold;
    private readonly CancellationToken _token;

    public FanControlService(
        ISensorReader sensor, 
        RelayController relay, 
        DataLogger logger, 
        TimeSpan interval,
        double humidityThreshold, 
        CancellationToken token)
    {
        _sensor = sensor;
        _relay = relay;
        _logger = logger;
        _timer = new PeriodicTimer(interval);
        _humidityThreshold = humidityThreshold;
        _token = token;
    }

    public async Task RunAsync()
    {
        try
        {
            while (await _timer.WaitForNextTickAsync(_token))
            {
                double temp = _sensor.ReadTemperature();
                double hum = _sensor.ReadHumidity();
                bool relayOn = hum > _humidityThreshold;

                _relay.SetRelay(relayOn);

                Console.WriteLine($"🌡️ {temp:F2}°C | 💧 {hum:F2}% | Relay: {(relayOn ? "ON" : "OFF")}");
                await _logger.LogAsync(temp, hum, relayOn);
            }
        }catch (OperationCanceledException)
        {
            Console.WriteLine("✅ Shutdown requested. Cleaning up...");
        }
    }
}