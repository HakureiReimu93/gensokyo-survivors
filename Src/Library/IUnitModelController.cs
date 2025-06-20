using System;

namespace GensokyoSurvivors.Src.Library;

public interface IUnitModelController
{
    public void ConsiderDamageInfo(ref float pRaw, UnitBuf[] bufs);
    public Godot.Vector2 MyPlannedMovement { get; }
}
