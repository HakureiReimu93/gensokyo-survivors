using System;
using Godot;
using GodotStrict.Helpers.Guard;
using GodotStrict.Types;
using GodotUtilities;

[GlobalClass]
[UseAutowiring]
[Icon("res://Assets/GodotEditor/Icons/visual.png")]
public partial class UnitMonoVisual : Node2D
{
	[Signal]
	public delegate void DeathAnimationFinishedEventHandler();

	[Autowired("Main")]
	Sprite2D mMain;

	[Autowired("UnitMonoAnim")]
	AnimationPlayer mAnim;
	private bool isFacingLeft;

	public override void _Ready()
	{
		__PerformDependencyInjection();

		SafeGuard.Ensure(mAnim.HasAnimation("die"));
		SafeGuard.Ensure(mAnim.HasAnimation("idle"));
		SafeGuard.Ensure(mAnim.HasAnimation("walk"));

		mAnim.AnimationFinished += HandleAnimationFinished;
	}

	public void DoPlayWalk()
	{
		SafeGuard.Ensure(mAnim.CurrentAnimation != "die");
		mAnim.Play("walk");
	}

	public void DoPlayIdle()
	{
		SafeGuard.Ensure(mAnim.CurrentAnimation != "die");
		mAnim.Play("idle");
	}

	public void DoPlayDie()
	{
		SafeGuard.Ensure(mHasPlayedDeathAnim.Never());
		mAnim.Play("die");
	}

	private void HandleAnimationFinished(StringName animName)
	{
		if (animName == "die")
		{
			EmitSignal(SignalName.DeathAnimationFinished);
		}
	}

	private EverFlag mHasPlayedDeathAnim;

	public bool IsFacingLeft
	{
		get
		{
			return isFacingLeft;
		}
		set
		{
			isFacingLeft = value;
			if (isFacingLeft)
			{
				Scale = new Vector2(-1, 1);
			}
			else
			{
				Scale = new Vector2(1, 1);
			}
		}
	}
}
