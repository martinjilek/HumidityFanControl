using HumidityFanControl.Config;
using HumidityFanControl.Core;
using HumidityFanControl.Core.Rules;
using HumidityFanControl.Core.States;
using HumidityFanControl.Data;
using HumidityFanControl.Data.Models;
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
    private readonly ILogRepository _logRepository;
    
    private readonly FanStateMachine _stateMachine;

    private bool _relayOn = false;
    private double _outsideHumidity = 0.0;
    private DateTime _lastRelayOnDatetime = DateTime.MinValue;
    private DateTime _lastRelayOffDatetime = DateTime.MinValue;
    private double _hum;
    private double _temp;

    private bool RelayOn
    {
        get => _relayOn;
        set
        {
            if (value && !_relayOn)
            {
                _lastRelayOnDatetime = DateTime.Now;
            }
            else
            {
                _lastRelayOffDatetime = DateTime.Now;
            }

            if (value != _relayOn)
            {
                _ = _logRepository.LogAsync(_temp, _hum, value, $"State changed to {(value ? "ON" : "OFF")}");
            }

            _relayOn = value;
            _relay.SetRelay(_relayOn);
        }
    }

    public FanControlService(
        RelayController relay, 
        ISensorService sensor, 
        IOptions<FanControlSettings> settings,
        IWeatherDataService weatherDataService, 
        ILogRepository logRepository)
    {
        _relay = relay;
        _sensor = sensor;
        _settings = settings.Value;
        _weatherDataService = weatherDataService;
        _logRepository = logRepository;
        
        _stateMachine = new FanStateMachine(new FanRule[]
        {
            new RunTimeTableRule(),
            new ScheduleRule(),
            new HumidityDifferenceRule(),
            new HumidityThresholdRule()
        });

        _weatherDataService.GetHumidityOnWeatherDataReceived += (data) => { _outsideHumidity = data; };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _temp = _sensor.ReadTemperature();
            _hum = _sensor.ReadHumidity();

            var ctx = new FanContext
            {
                CurrentTime = DateTime.Now,
                InsideHumidity = _hum,
                InsideTemperature = _temp,
                OutsideHumidity = _outsideHumidity
            };

            var newState = _stateMachine.Next(ctx, _settings);
            RelayOn = _stateMachine.RelayOn;
            
            Console.WriteLine(
                $"🌡️ {_temp:F2}°C | 💧 {_hum:F2}% | State: {newState} | Relay: {(RelayOn ? "ON" : "OFF")} | Outside Humidity: {_outsideHumidity:F2}%");

            await Task.Delay(_settings.ReadIntervalMs, stoppingToken);
        }
    }
}