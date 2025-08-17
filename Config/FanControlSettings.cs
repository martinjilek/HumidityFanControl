namespace HumidityFanControl.Config;

public class FanControlSettings
{
    public double HumidityThreshold { get; set; }
    public int RelayGpioPin { get; set; }
    public int ReadIntervalMs { get; set; }
    public Dictionary<string, List<ScheduleInterval>?>? Schedule { get; set; }
}

public class ScheduleInterval
{
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool Enabled { get; set; }
}

