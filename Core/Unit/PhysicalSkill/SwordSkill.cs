using GensokyoSurvivors.Core.Interface;
using Godot;
using GodotStrict.Helpers.Dependency;
using GodotStrict.Helpers.Guard;
using GodotStrict.Helpers.Logging;
using GodotUtilities;
using static GodotStrict.Helpers.Logging.StrictLog;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/skill.png")]
public partial class SwordSkill : Node2D
{
	AnimationPlayer mAnim;

	public override void _Ready()
	{
		mAnim = this.Require<AnimationPlayer>();

		SafeGuard.Ensure(mAnim.HasAnimation("default"));

		mAnim.Play("default");
	}
}
