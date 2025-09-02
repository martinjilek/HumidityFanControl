using HumidityFanControl.Config;

namespace HumidityFanControl.Core.Rules;

public class HumidityDifferenceRule : FanRule
{
    public override string Name => "Humidity Difference Rule";
    public override bool Active => false;

    protected override bool EvaluateRule(FanContext context, FanControlSettings settings)
    {
        if (context.OutsideHumidity == 0.0) return true; // no data → allow
        return context.OutsideHumidity < context.InsideHumidity;
    }
}