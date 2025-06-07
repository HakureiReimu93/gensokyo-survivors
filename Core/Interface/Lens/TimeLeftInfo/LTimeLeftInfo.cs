using System;
using Godot;
using GodotStrict.Traits;
using GodotStrict.Types;
using Microsoft.CodeAnalysis;

namespace GensokyoSurvivors.Core.Presentation.UI.TimeDisplay;

public interface LTimeLeftInfo: ILens<Node>
{
    public Option<TimeBundle> GetTime();
}
