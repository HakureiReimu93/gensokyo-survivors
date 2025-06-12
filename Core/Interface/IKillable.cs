using System;

namespace GensokyoSurvivors.Core.Interface;

public interface IKillable
{
    public void TriggerDie();
    public void TriggerDieForcely();
    public void OnDie(Action pContinuation);
    public bool IsDead { get; }
}
