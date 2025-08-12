using System.Device.Gpio;
using HumidityFanControl.Config;
using Microsoft.Extensions.Options;

namespace HumidityFanControl.Hardware;

public class RelayController : IDisposable
{
    private readonly FanControlSettings _settings;
    private readonly GpioController _gpio;
    
    public RelayController(IOptions<FanControlSettings> options)
    {
        _settings = options.Value;
        _gpio = new GpioController();
        _gpio.OpenPin(_settings.RelayGpioPin, PinMode.Output);
    }
    
    public void SetRelay(bool on) =>
        _gpio.Write(_settings.RelayGpioPin, on ? PinValue.High : PinValue.Low);
    
    public void Dispose()
    {
        Console.WriteLine("🗑️ Disposing RelayController...");
        _gpio.Write(_settings.RelayGpioPin, PinValue.Low);
        _gpio.ClosePin(_settings.RelayGpioPin);
    }
    
}