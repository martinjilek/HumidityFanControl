namespace HumidityFanControl.Data.Models;
public class SensorLog
{
    public int Id { get; set; }
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public bool FanStateOn { get; set; }
    public string Note { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; } = DateTime.Now;
}