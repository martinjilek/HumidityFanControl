using HumidityFanControl.Config;
using Microsoft.Extensions.Logging;

namespace HumidityFanControl.Core.Rules;

public class RunTimeTableRule : FanRule
{
    public override string Name => "Run time table Rule";

    protected override bool EvaluateRule(FanContext context, FanControlSettings settings)
    {
        var now = context.CurrentTime.TimeOfDay;
        FanControlInterval? defaultInterval = null;

        var schedule = settings.RunTimeTable;
        // if no schedule defined, allow running at any time, otherwise run ONLY during defined intervals
        if (schedule == null || schedule.Count == 0)
            return true;

        schedule.TryGetValue(context.CurrentTime.DayOfWeek.ToString(), out var intervals);
        if (intervals == null || intervals.Count == 0)
            schedule.TryGetValue("Default", out intervals);

        if (intervals == null || intervals.Count == 0)
        {
            return false;
        }

        return intervals.Any(i => now >= i.StartTime && now <= i.EndTime);
    }
}