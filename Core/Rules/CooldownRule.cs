using HumidityFanControl.Config;

namespace HumidityFanControl.Core.Rules;

public class CooldownRule : FanRule
{
    public override string Name => "Cooldown Rule";

    protected override bool EvaluateRule(FanContext context, FanControlSettings settings)
    {
        if (context.LastTurnedOn.HasValue)
        {
            var runtime = context.CurrentTime - context.LastTurnedOn.Value;

            // Must stay ON until min runtime has elapsed
            if (runtime < TimeSpan.FromMinutes(settings.FanTimingSettings.MinOnTimeMinutes))
                return true;

            // If it's been running longer than MaxOnTime, force OFF
            if (runtime > TimeSpan.FromMinutes(settings.FanTimingSettings.MaxOnTimeMinutes))
                return false;
        }

        if (context.LastTurnedOff.HasValue)
        {
            var downtime = context.CurrentTime - context.LastTurnedOff.Value;

            // If OFF less than cooldown, block restart
            if (downtime < TimeSpan.FromMinutes(settings.FanTimingSettings.CooldownTimeMinutes))
                return false;
        }

        return true;
    }
}