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
    private DateTime _lastFanOn = DateTime.MinValue;
    private double _hum;
    private double _temp;

    private bool RelayOn
    {
        get => _relayOn;
        set
        {
            if (value && !_relayOn)
            {
                _lastFanOn = DateTime.Now;
            }

            _relayOn = value;
            _relay.SetRelay(_relayOn);
        }
    }




    public FanControlService(RelayController relay, ISensorService sensor, IOptions<FanControlSettings> settings,
        IWeatherDataService weatherDataService)
    {
        _relay = relay;
        _sensor = sensor;
        _settings = settings.Value;
        _weatherDataService = weatherDataService;

        _weatherDataService.GetHumidityOnWeatherDataReceived += (data) => { _outsideHumidity = data; };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _temp = _sensor.ReadTemperature();
            _hum = _sensor.ReadHumidity();

            RelayOn = _hum > _settings.HumidityThreshold;

            Console.WriteLine(
                $"🌡️ {_temp:F2}°C | 💧 {_hum:F2}% | Relay: {(RelayOn ? "ON" : "OFF")} | Outside Humidity: {_outsideHumidity:F2}%");

            await Task.Delay(_settings.ReadIntervalMs, stoppingToken);
        }
    }

    /// <summary>
    /// Checks if current time is within the configured schedule.
    /// 
    /// If no schedule is set, run the default schedule
    /// If no default is set then the schedule is 9:00 to 19:00
    /// </summary>
    /// <returns>bool</returns>
    private bool CheckSchedule()
    {
        var now = DateTime.Now.TimeOfDay;
        var reasonableDefault = new ScheduleInterval
        {
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(19, 0, 0)
        };

        try
        {
            var schedule = _settings?.Schedule;
            if (schedule == null || schedule.Count == 0)
            {
                return now < reasonableDefault.StartTime && reasonableDefault.EndTime > now;
            }

            schedule.TryGetValue(DateTime.Now.DayOfWeek.ToString(), out var intervals);

            if (intervals == null || intervals.Count == 0)
            {
                // empty intervals, get the default interval
                intervals = schedule["default"];

                if (intervals == null || intervals.Count == 0)
                {
                    return now < reasonableDefault.StartTime && reasonableDefault.EndTime > now;
                }
            }

            return intervals.Any(i => i.StartTime >= now && i.EndTime <= now);

        }
        catch (Exception e)
        {
            return now < reasonableDefault.StartTime && reasonableDefault.EndTime > now;
        }
    }

}