using GodotStrict.AliasTypes;

namespace GensokyoSurvivors.Core.Model;

public record class ExperienceBundle(uint CurrentExp, uint MaxExp, uint Level)
{
    public normal PercentToNextLevel()
    {
        return new normal((float)CurrentExp / MaxExp);
    }
}
