using System;
using Godot;

namespace GensokyoSurvivors.Source.Library.Common;

public interface IPilot
{
    public EntityUnit GetUnit();
    public Vector2 CalculateMovement();
}
