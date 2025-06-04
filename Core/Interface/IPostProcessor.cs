using System;
using Godot;
using GodotStrict.AliasTypes;

namespace GensokyoSurvivors.Core.Interface;

public interface IScalarMiddleware<T>
{
    public normal NextValue(T pDirection, double delta);
}
