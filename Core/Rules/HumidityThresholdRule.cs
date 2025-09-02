using HumidityFanControl.Config;

namespace HumidityFanControl.Core.Rules;

public class HumidityThresholdRule : FanRule
{
    protected override bool EvaluateRule(FanContext context, FanControlSettings settings) =>
        context.InsideHumidity >= settings.HumidityThreshold;
    public override bool Active => false;

    public override string Name => "Humidity Threshold Rule";
}