using HumidityFanControl.Data.Models;

namespace HumidityFanControl.Data;

public interface ILogRepository
{
    Task LogAsync(double temperature, double humidity, bool relayOn, string note = "");
}

