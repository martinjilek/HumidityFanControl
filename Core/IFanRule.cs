using HumidityFanControl.Config;

namespace HumidityFanControl.Core;


public interface IFanRule
{
    bool CanRun(FanContext context, FanControlSettings settings);
    string Name { get; }
}