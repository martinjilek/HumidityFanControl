using HumidityFanControl.Config;

namespace HumidityFanControl.Core.Rules;

public class ScheduleRule : IFanRule
{
    public string Name => "Schedule Rule";

    public bool CanRun(FanContext context, FanControlSettings settings)
    {
        var now = context.CurrentTime.TimeOfDay;
        
        var defaultInterval = new ScheduleInterval { StartTime = new TimeSpan(9,0,0), EndTime = new TimeSpan(19,0,0) };

        var schedule = settings.Schedule;
        if (schedule == null || schedule.Count == 0)
            return now >= defaultInterval.StartTime && now <= defaultInterval.EndTime;

        schedule.TryGetValue(context.CurrentTime.DayOfWeek.ToString(), out var intervals);
        if (intervals == null || intervals.Count == 0)
            schedule.TryGetValue("default", out intervals);

        if (intervals == null || intervals.Count == 0)
            return now >= defaultInterval.StartTime && now <= defaultInterval.EndTime;

        return intervals.Any(i => now >= i.StartTime && now <= i.EndTime);
    }
}