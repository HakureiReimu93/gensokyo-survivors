using System;
using Godot;

namespace GensokyoSurvivors.Core.Interface;

public interface IVelocityMiddleware
{
    public Vector2 GetNextVelocity(Vector2 pDirection, double delta);
    public void DoSetSpeedForcely(float pSpeed);
}
