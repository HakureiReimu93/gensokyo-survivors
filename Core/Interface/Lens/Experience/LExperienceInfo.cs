using System;
using GensokyoSurvivors.Core.Model;
using Godot;
using GodotStrict.Traits;

namespace GensokyoSurvivors.Core.Interface.Lens.Experience;

public interface LExperienceInfo : ILens<Node>
{
    public TimeBundle GetExperience();
}