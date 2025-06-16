using System;
using System.Collections.Generic;
using Godot;
using GodotStrict.Types;

namespace GensokyoSurvivors.Core.Interface;

public interface IAdversarialUnitPicker
{
    Option<MobUnit> ComputeUnitVictim();
    IAdversarialUnitPicker WithProtagonist(MobUnit protagonist);
    IAdversarialUnitPicker WithAntagonists(IEnumerable<MobUnit> antagonists);
}

public interface IPickPointArountProtagonist
{
    Vector2 ComputeOffset();
    IPickPointArountProtagonist WithProtagonist(MobUnit protagonist);
}
