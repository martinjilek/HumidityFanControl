using HumidityFanControl.Config;

namespace HumidityFanControl.Core;

public abstract class FanRule
{
    public virtual bool Active { get; set; } = true;    
    public bool CanRun(FanContext context, FanControlSettings settings)
    {
        if (!Active) return true;
        return EvaluateRule(context, settings);
    }
    
    protected abstract bool EvaluateRule(FanContext context, FanControlSettings settings);
    
    public abstract string Name { get; }
}