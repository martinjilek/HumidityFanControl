namespace HumidityFanControl.Hardware;

public interface ISensorService
{
    double ReadTemperature();
    double ReadHumidity();
}