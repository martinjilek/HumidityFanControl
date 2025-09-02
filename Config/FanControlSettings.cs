namespace HumidityFanControl.Config;

public class FanControlSettings
{
    public double HumidityThreshold { get; set; }
    public int RelayGpioPin { get; set; }
    public int ReadIntervalMs { get; set; }
    public TimingSettings FanTimingSettings { get; set; }
    public Dictionary<string, List<FanControlInterval>?>? Schedule { get; set; }
    
    public Dictionary<string, List<FanControlInterval?>?>? RunTimeTable { get; set; }
}

public class FanControlInterval
{
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool Enabled { get; set; } = true;
}

public class TimingSettings
{
    public int MinOnTimeMinutes { get; set; }
    public int MaxOnTimeMinutes { get; set; }
    public int CooldownTimeMinutes { get; set; }
}

