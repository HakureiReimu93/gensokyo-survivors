using GodotStrict.AliasTypes;

namespace GensokyoSurvivors.Core.Model;

public record class TimeBundle(double TimeLeft, double MaxTime)
{
    public normal PercentTimeLeft()
    {
        return new normal(TimeLeft / MaxTime);
    }
}
