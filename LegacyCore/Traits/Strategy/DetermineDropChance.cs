using Godot;
using System;
using static Godot.Mathf;
using static GodotStrict.Helpers.Logging.StrictLog;
using static GodotStrict.Helpers.Dependency.DependencyHelper;
using GodotStrict.Helpers;
using GodotStrict.Helpers.Guard;
using GensokyoSurvivors.Core.Interface;
using GodotStrict.AliasTypes;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/equation.png")]
public partial class DetermineDropChance : Node2D, IDropProbabilityCalculator
{
	public bool WillDrop()
	{
		return Calculate.ChanceOf(new normal(0.3f));
	}
}