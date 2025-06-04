using System;

namespace GensokyoSurvivors.Core.Interface;

public interface IPhysicalSkill
{
    public void OnEnemyHit(MobUnit enemy);
    public void OnEnemyChosen(MobUnit enemy);
}
