using System;
using Godot;

namespace GensokyoSurvivors.Src.Library.Midware;

[GlobalClass]
[Icon("res://GodotEditor/Icons/equation.png")]
public abstract partial class UnitMidWare : Resource, IUnitModelMidware
{
    public abstract void Process(UnitModel pUnit, double delta);
}
