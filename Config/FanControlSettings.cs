namespace HumidityFanControl.Config;

public class FanControlSettings
{
    public double HumidityThreshold { get; set; }
    public int RelayGpioPin { get; set; }
    public double ReadIntervalMs { get; set; }
}