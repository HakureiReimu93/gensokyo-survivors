using System;
using GensokyoSurvivors.Core.Utility;
using Godot;
using GodotStrict.Traits;

namespace GensokyoSurvivors.Core.Interface.Lens;

public interface IPickupCommandDispatcher
{
    public Outcome SendExpReward(int pExperienceGainedRaw);
}

public interface IPickupCommandReceiver
{
    public void ReceiveExpReward(int pExperienceGainedRaw);
}
