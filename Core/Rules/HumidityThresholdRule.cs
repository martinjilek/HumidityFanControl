using HumidityFanControl.Config;

namespace HumidityFanControl.Core.Rules;

public class HumidityThresholdRule : IFanRule
{
    public string Name => "Humidity Threshold Rule";

    public bool CanRun(FanContext context, FanControlSettings settings)
        => context.InsideHumidity >= settings.HumidityThreshold;
}