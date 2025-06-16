using Godot;
using System;
using static Godot.Mathf;
using static GodotStrict.Helpers.Logging.StrictLog;
using static GodotStrict.Helpers.Dependency.DependencyHelper;
using GodotStrict.Helpers;
using GodotStrict.Helpers.Guard;
using GensokyoSurvivors.Core.Interface.Lens.Experience;
using GensokyoSurvivors.Core.Model;
using GodotStrict.Traits;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/script.png")]
public partial class ExperienceBarUI : CanvasLayer, LExperienceChannel
{
	[Export]
	ProgressBar MyProgressBar;

	public override void _Ready()
	{
		SafeGuard.EnsureNotNull(MyProgressBar);
		SafeGuard.Ensure(MyProgressBar.MinValue == 0);
		SafeGuard.Ensure(MyProgressBar.MaxValue == 100);
	}

	public void ReceiveExperience(ExperienceBundle pInput)
	{
		MyProgressBar.Value = pInput.PercentToNextLevel() * 100;
	}

	Node ILens<Node>.Entity => this;
}
