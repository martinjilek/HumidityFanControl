using System.Device.I2c;

namespace HumidityFanControl.Hardware;

public class Htu21dSensorReader : ISensorReader
{
    private readonly I2cDevice _device;
    private const byte TRIGGER_TEMP_MEASURE_HOLD = 0xE3;
    const byte TRIGGER_HUMD_MEASURE_HOLD = 0xE5;

    public Htu21dSensorReader(I2cDevice device)
    {
        _device = device;
    }
    
    public double ReadTemperature()
    {
        Span<byte> cmd = stackalloc byte[] { TRIGGER_TEMP_MEASURE_HOLD };
        Span<byte> data = stackalloc byte[3];

        _device.Write(cmd);
        Thread.Sleep(50);
        _device.Read(data);

        int raw = (data[0] << 8) | (data[1] & 0xFC);
        return -46.85 + 175.72 * raw / 65536.0;
    }
    
    public double ReadHumidity()
    {
        Span<byte> cmd = stackalloc byte[] { TRIGGER_HUMD_MEASURE_HOLD };
        Span<byte> data = stackalloc byte[3];

        _device.Write(cmd);
        Thread.Sleep(50);
        _device.Read(data);

        int raw = (data[0] << 8) | (data[1] & 0xFC);
        return -6 + 125.0 * raw / 65536.0;
    }
    
}