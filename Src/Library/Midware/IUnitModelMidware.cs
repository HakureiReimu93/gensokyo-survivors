using System;

namespace GensokyoSurvivors.Src.Library.Midware;

/// <summary>
/// Middleware is different from bufs in that they are permanent and always active.
/// They are used to 'style' an entity in terms of appearance, movement behavior, etc.
/// Not very common.
/// </summary>
public interface IUnitModelMidware
{
    public void Process(UnitModel pUnit, double delta);
}
