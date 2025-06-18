using System;
using Godot;

namespace GensokyoSurvivors.Source.Library.Common;

/// <summary>
/// Disable me by setting my process mode to disabled.
/// </summary>
public interface IVisualMidWare
{
    public void ActivateForSeconds(float duration=0);
    public void NextEffect(CanvasItem input, double delta);
}
