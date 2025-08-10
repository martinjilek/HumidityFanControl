namespace HumidityFanControl.Hardware;

public interface ISensorReader
{
    double ReadTemperature();
    double ReadHumidity();
}