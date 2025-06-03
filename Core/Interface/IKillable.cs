using System;

namespace GensokyoSurvivors.Core.Interface;

public interface IKillable
{
    public void TriggerDie();
    public bool IsDead { get; }
}
