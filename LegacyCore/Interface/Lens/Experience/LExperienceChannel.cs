using System;
using GensokyoSurvivors.Core.Model;
using Godot;
using GodotStrict.Traits;

namespace GensokyoSurvivors.Core.Interface.Lens.Experience;

public interface LExperienceChannel : ILens<Node>
{
    public void ReceiveExperience(ExperienceBundle pInput);
}

