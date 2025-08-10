using HumidityFanControl.Models;

namespace HumidityFanControl.Data;

public class DataLogger
{
    private readonly HfcContext _context;

    public DataLogger(HfcContext context)
    {
        _context = context;
    }

    public async Task LogAsync(double temp, double hum, bool relayOn)
    {
        _context.Logs.Add(new()
        {
            FanStateOn = relayOn,
            DateCreated = DateTime.Now,
            Humidity = hum,
            Temperature = temp,
        });
        await _context.SaveChangesAsync();
    }
}