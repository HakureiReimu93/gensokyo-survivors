using Godot;
using System;
using static Godot.Mathf;
using static GodotStrict.Helpers.Logging.StrictLog;
using static GodotStrict.Helpers.Dependency.DependencyHelper;
using GodotStrict.Helpers;
using GodotStrict.Helpers.Guard;
using GodotUtilities;
using GensokyoSurvivors.Core.Utility;
using GodotStrict.Types;

[GlobalClass]
[UseAutowiring]
[Icon("res://Assets/GodotEditor/Icons/script.png")]
public partial class ExperienceVialUnit : Node2D
{
	[Autowired]
	UnitMonoVisual mVisuals;

	[Autowired("CollectEnterBox")]
	EnterBox mCollectListener;

	[Autowired("GravitateEnterBox")]
	EnterBox mGravitateListener;

	public override void _Ready()
	{
		__PerformDependencyInjection();

		mCollectListener.EnterBoxEntered += OnCollectionRegionEnter;
		mGravitateListener.EnterBoxEntered += OnGravitateRegionEnter;

		mVisuals.DoRegisterFallbackAnim("idle")
				.DoRegisterLoopingAnim("chase")
				.DoRegisterOneShotLockedAnim("spawn");

		mState.WithOwner(this)
				.PlanRoute(DoIdleRoutine, DoEnterIdleRoutine)
				.PlanRoute(DoChaseRoutine);

		mVisuals.TryPlayAnimationAndAwaitCompletion("spawn", out mSpawnAnimAwaiter);
		mSpawnAnimAwaiter.OnCompleted(
			() => mState.GoTo(DoIdleRoutine)
		);

		mCollectListener.SetEnabled(false);
		mGravitateListener.SetEnabled(false);

		SafeGuard.Ensure(mCollectListener.MyFaction == mGravitateListener.MyFaction);
	}

	public override void _Process(double delta)
	{
		mState.Process(delta);
	}

	private void DoEnterIdleRoutine(double delta)
	{
		mCollectListener.SetEnabled(true);
		mGravitateListener.SetEnabled(true);

		
	}

	private void DoIdleRoutine(double delta)
	{

	}

	private void DoChaseRoutine(double delta)
	{

	}

	private void OnCollectionRegionEnter()
	{

	}

	private void OnGravitateRegionEnter()
	{

	}

	

	AnimSoon mSpawnAnimAwaiter;
	LiteStates mState;
}
