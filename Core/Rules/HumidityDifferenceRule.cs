using HumidityFanControl.Config;

namespace HumidityFanControl.Core.Rules;

public class HumidityDifferenceRule : IFanRule
{
    public string Name => "Humidity Difference Rule";

    public bool CanRun(FanContext context, FanControlSettings settings)
    {
        if (context.OutsideHumidity == 0.0) return true; // no data → allow
        return context.OutsideHumidity < context.InsideHumidity;
    }
}