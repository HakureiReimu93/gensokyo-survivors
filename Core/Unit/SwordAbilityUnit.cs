using System;
using GensokyoSurvivors.Core.Interface;
using Godot;
using GodotStrict.Helpers.Guard;
using GodotStrict.Types;
using GodotUtilities;
using static GodotStrict.Helpers.Logging.StrictLog;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/skill-blue.png")]
[UseAutowiring]
public partial class SwordAbilityUnit : Node2D, IPhysicalSkill
{
	[Autowired("AppliedOnVictimChosen")]
	Option<UnitBuf> mAnticipateBufTemplate;

	[Autowired]
	AnimationPlayer mAnim;


	public override void _Ready()
	{
		__PerformDependencyInjection();

		SafeGuard.Ensure(mAnim.HasAnimation("default"));
		mAnim.Play("default");
		mAnim.AnimationFinished += OnAnimationEnd;
	}

	private void OnAnimationEnd(StringName animName)
	{
		QueueFree();

		// remove buf that was slapped on top of enemy.
		if (mAnticipateBufInstance.Available(out var buf) &&
			mTarget.Available(out var target))
		{
			target.RemoveUnitBuf(buf);
		}
	}

	public void OnEnemyChosen(MobUnit enemy)
	{
		if (mAnticipateBufTemplate.Unavailable(out var template)) return;
		mAnticipateBufTemplate = template.DoCloneMe();
		enemy.AddUnitBuf(template);

		// remember unit buf
		mAnticipateBufInstance = template;

		// remember target
		mTarget = enemy; 

		// Rotate the blade
		LookAt(enemy.GlobalPosition);
	}

	public void OnEnemyHit(MobUnit enemy)
	{
		LogAny("I hit an enemy with a sword. His name was: " + enemy.Name);
	}

	Option<UnitBuf> mAnticipateBufInstance = Option<UnitBuf>.None;
	Option<MobUnit> mTarget = Option<MobUnit>.None;
}
