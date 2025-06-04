using GensokyoSurvivors.Core.Interface;
using Godot;
using GodotStrict.Helpers.Dependency;
using GodotStrict.Helpers.Guard;
using GodotStrict.Helpers.Logging;
using GodotStrict.Types;
using GodotUtilities;
using Microsoft.CodeAnalysis;
using static GodotStrict.Helpers.Logging.StrictLog;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/skill-blue.png")]
[UseAutowiring]
public partial class SwordAbilityUnit : Node2D, IPhysicalSkill
{
	[Autowired("AppliedOnChooseTarget")]
	Option<UnitBuf> mApplyOnChooseTarget;

	[Autowired]
	AnimationPlayer mAnim;

	public override void _Ready()
	{
		SafeGuard.Ensure(mAnim.HasAnimation("default"));
		mAnim.Play("default");
	}

	public void OnEnemyChosen(MobUnit enemy)
	{
	}

	public void OnEnemyHit(MobUnit enemy)
	{
		LogAny("I hit an enemy with a sword. His name was: " + enemy.Name);
	}


	// Run dependency injection before _Ready() is called.
	public override void _Notification(int what) { if (what == NotificationSceneInstantiated) __PerformDependencyInjection(); }	
}
