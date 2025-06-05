using System;
using Godot;
using GodotStrict.Traits;

namespace GensokyoSurvivors.Core.Interface.Lens;

public interface LBoundsInfo2D : LInfo2D
{
    public Rect2 GetBounds();
}
