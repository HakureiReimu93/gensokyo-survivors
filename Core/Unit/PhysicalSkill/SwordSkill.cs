using GensokyoSurvivors.Core.Interface;
using Godot;
using GodotStrict.Helpers.Logging;
using GodotUtilities;
using static GodotStrict.Helpers.Logging.StrictLog;

[GlobalClass]
public partial class SwordSkill : Node2D, IPhysicalSkill
{

	public override void _Ready()
	{
		base._Ready();
	}

	public void OnHitEnemy()
	{
		LogAny("I attacked an enemy!");
	}
}
