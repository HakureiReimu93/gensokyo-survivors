using GensokyoSurvivors.Core.Interface;
using Godot;
using GodotStrict.Helpers;
using GodotStrict.Helpers.Guard;
using System;

public partial class SpawnAroundPlayer : Node, IPickPointArountProtagonist
{
    public Vector2 ComputeOffset()
    {
        var result = mProtagonist.GlobalPosition + Calculate.RandomDirection() * 40f;
        mProtagonist = null;
        return result;
    }

    public IPickPointArountProtagonist WithProtagonist(MobUnit protagonist)
    {
        SafeGuard.Ensure(mProtagonist is null);
        mProtagonist = protagonist;
        return this;
    }

    MobUnit mProtagonist;
}
