using GensokyoSurvivors.Core.Interface;
using Godot;
using GodotStrict.Helpers.Dependency;
using GodotStrict.Helpers.Guard;
using GodotStrict.Helpers.Logging;
using GodotUtilities;
using static GodotStrict.Helpers.Logging.StrictLog;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/skill-blue.png")]
public partial class SwordAbilityUnit : Node2D, IPhysicalSkill
{
	AnimationPlayer mAnim;

	public void OnHitEnemy()
	{
		LogAny("I hit an enemy with a sword");
	}

	public override void _Ready()
	{
		mAnim = this.Require<AnimationPlayer>();

		SafeGuard.Ensure(mAnim.HasAnimation("default"));

		mAnim.Play("default");
	}
}
