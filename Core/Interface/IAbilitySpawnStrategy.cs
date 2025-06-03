using System;
using System.Collections.Generic;
using Godot;
using GodotStrict.Types;

namespace GensokyoSurvivors.Core.Interface;

public interface IAbilitySpawnStrategy
{
    Option<Vector2> ComputeSpawnOrigin();
    IAbilitySpawnStrategy WithProtagonistPosition(Vector2 origin);
    IAbilitySpawnStrategy WithAntagonistPositions(IEnumerable<Vector2> origins);
}
