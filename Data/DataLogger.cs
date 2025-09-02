using HumidityFanControl.Data.Models;

namespace HumidityFanControl.Data;

public class DataLogger : ILogRepository
{
    private readonly HfcContext _context;

    public DataLogger(HfcContext context)
    {
        _context = context;
    }

    public async Task LogAsync(double temperature, double humidity, bool relayOn, string note = "")
    {
        try
        {
            _context.Logs.Add(new SensorLog
            {
                FanStateOn = relayOn,
                DateCreated = DateTime.UtcNow,
                Humidity = humidity,
                Temperature = temperature,
                Note = note
            });
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error logging data: {e.Message}");
        }
        
    }
}