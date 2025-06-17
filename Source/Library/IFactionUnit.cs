using System;

namespace GensokyoSurvivors.Source.Library;

public interface IFactionUnit
{
    public void SetFaction(Faction pFaction);
    public Faction MyFaction { get; }
}
