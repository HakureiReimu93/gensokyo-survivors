using System;
using Godot;

namespace GensokyoSurvivors.Source.Library.Common;

public interface IValueBuf<T>
where T: struct
{
    public void CalculateValue(ref Vector2 input);
}
