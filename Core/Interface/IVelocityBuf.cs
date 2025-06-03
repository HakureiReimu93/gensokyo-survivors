using System;
using Godot;
using GodotStrict.AliasTypes;

namespace GensokyoSurvivors.Core.Interface;

/// <summary>
/// Interface used to modify speed (by percent).
/// </summary>
public interface IVelocityBuf
{
    public Vector2 GetVelocityBuf(Vector2 pDirection, float baseSpeed, double delta);
}
