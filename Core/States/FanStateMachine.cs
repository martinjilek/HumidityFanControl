using HumidityFanControl.Config;

namespace HumidityFanControl.Core.States;

public class FanStateMachine
{
    private FanState _state = FanState.Off;
    private DateTime _stateEnteredAt = DateTime.MinValue;
    private readonly IEnumerable<FanRule> _rules;

    public FanStateMachine(IEnumerable<FanRule> rules)
    {
        _rules = rules;
    }

    public FanState CurrentState => _state;

    public bool RelayOn => _state == FanState.Running;

    public FanState Next(FanContext ctx, FanControlSettings settings)
    {
        switch (_state)
        {
            case FanState.Off:
                if (AllRulesPass(ctx, settings))
                {
                    _state = FanState.Running;
                    _stateEnteredAt = ctx.CurrentTime;
                }
                break;

            case FanState.Running:
                var runtime = ctx.CurrentTime - _stateEnteredAt;

                // Must run for at least MinOnTime
                if (runtime < TimeSpan.FromMinutes(settings.FanTimingSettings.MinOnTimeMinutes))
                    break;

                // Too long? -> force cooldown
                if (runtime > TimeSpan.FromMinutes(settings.FanTimingSettings.MaxOnTimeMinutes))
                {
                    _state = FanState.CoolingDown;
                    _stateEnteredAt = ctx.CurrentTime;
                    break;
                }

                // Conditions no longer met? -> stop
                if (!AllRulesPass(ctx, settings))
                {
                    _state = FanState.Off;
                    _stateEnteredAt = ctx.CurrentTime;
                }
                break;

            case FanState.CoolingDown:
                var downtime = ctx.CurrentTime - _stateEnteredAt;
                if (downtime > TimeSpan.FromMinutes(settings.FanTimingSettings.CooldownTimeMinutes))
                {
                    _state = FanState.Off;
                    _stateEnteredAt = ctx.CurrentTime;
                }
                break;
        }

        return _state;
    }

    private bool AllRulesPass(FanContext ctx, FanControlSettings settings)
    {
        foreach (var rule in _rules)
        {
            if (!rule.CanRun(ctx, settings))
            {
                Console.WriteLine($"❌ Rule blocked: {rule.Name}");
                return false;
            }
        }
        return true;
    }
}