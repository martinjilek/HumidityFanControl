using System.Device.Gpio;

namespace HumidityFanControl.Hardware;

public class RelayController : IDisposable
{
    private readonly GpioController _gpio;
    private readonly int _pin;
    
    public RelayController(GpioController gpio, int pin)
    {
        _gpio = gpio;
        _pin = pin;
        _gpio.OpenPin(_pin, PinMode.Output);
    }
    
    public void SetRelay(bool on) =>
        _gpio.Write(_pin, on ? PinValue.High : PinValue.Low);
    
    public void Dispose()
    {
        _gpio.Write(_pin, PinValue.Low);
        _gpio.ClosePin(_pin);
    }
    
}