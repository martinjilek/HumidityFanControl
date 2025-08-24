namespace HumidityFanControl.Core;

public class FanContext
{
    public DateTime CurrentTime { get; set; }
    public double InsideHumidity { get; set; }
    public double OutsideHumidity { get; set; }
    public double InsideTemperature { get; set; }
    public DateTime? LastTurnedOn { get; set; }
    public DateTime? LastTurnedOff { get; set; }
}