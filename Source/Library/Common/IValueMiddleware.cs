using System;
using Godot;

namespace GensokyoSurvivors.Source.Library.Common;

public interface IValueMidWare<T>
where T: struct
{
    public void CalculateValue(ref T input, double delta);
}
