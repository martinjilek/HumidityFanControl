using HumidityFanControl.Config;

namespace HumidityFanControl.Core;

public class FanDecisionEngine
{
    private readonly List<FanRule> _rules;

    public FanDecisionEngine(IEnumerable<FanRule> rules)
    {
        _rules = rules.ToList();
    }

    public bool ShouldRun(FanContext context, FanControlSettings settings)
    {
        foreach (var rule in _rules)
        {
            if (!rule.CanRun(context, settings))
            {
                Console.WriteLine($"Rule blocked: {rule.Name}");
                return false;
            }
        }
        return true;
    }
}