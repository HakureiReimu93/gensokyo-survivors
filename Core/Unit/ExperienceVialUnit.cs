using Godot;
using GodotStrict.Helpers;
using GodotStrict.Helpers.Guard;
using GodotUtilities;
using GensokyoSurvivors.Core.Utility;
using GodotStrict.Types;
using GensokyoSurvivors.Core.Interface.Lens;
using GodotStrict.Helpers.Logging;
using GodotStrict.Traits;
using GensokyoSurvivors.Core.Interface;

[GlobalClass]
[UseAutowiring]
[Icon("res://Assets/GodotEditor/Icons/script.png")]
public partial class ExperienceVialUnit : Node2D, IDesignToken<ExperienceVialUnit>
{
	[Autowired]
	UnitMonoVisual mVisuals;

	[Autowired("CollectEnterBox")]
	EnterBox mCollectListener;

	[Autowired("ChaseEnterBox")]
	EnterBox mGravitateListener;

	public override void _Ready()
	{
		__PerformDependencyInjection();

		SafeGuard.Ensure(MyChaseSpeed > 0);
		SafeGuard.Ensure(MyExperiencePoints > 0);

		mCollectListener.AreaEntered += OnCollectionRegionEnter;
		mGravitateListener.AreaEntered += OnChaseRegionEnter;

		mVisuals.DoRegisterFallbackAnim("idle")
				.DoRegisterLoopingAnim("chase")
				.DoRegisterOneShotLockedAnim("spawn");

		mState.WithOwner(this)
				.PlanRoute(DoChaseRoutine, DoEnterChaseRoutine);

		mVisuals.TryPlayAnimationAndAwaitCompletion("spawn", out mSpawnAnimAwaiter);
		mSpawnAnimAwaiter.OnCompleted(DoEnterIdleRoutine);

		mCollectListener.SetEnabled(false);
		mGravitateListener.SetEnabled(false);

		SafeGuard.Ensure(mCollectListener.MyFaction == mGravitateListener.MyFaction);

		// Disable, since this is treated as a design token.
		if (ActivateAtStart is false)
		{
			Visible = false;
			ProcessMode = ProcessModeEnum.Disabled;
		}
	}

	public void Activate()
	{
		Visible = true;
		ProcessMode = ProcessModeEnum.Inherit;
	}

	public ExperienceVialUnit Duplicate()
	{
		return this.Duplicate();
	}

	public override void _Process(double delta)
	{
		mState.Process(delta);
	}

	private void DoEnterIdleRoutine()
	{
		mCollectListener.SetEnabled(true);
		mGravitateListener.SetEnabled(true);

		mVisuals.TryPlayAnimation("idle");

		// Randomize the animation so that groups of experience vials don't have synced animations
		mVisuals.TrySetPlayHeadToPercentDone(Calculate.RandomPercent());
	}

	private void DoChaseRoutine(double delta)
	{
		if (mTargetInfo2D.Unavailable(out var target2D)) return;

		var directionToTarget = GlobalPosition.DirectionTo(target2D.GlobalPosition);
		GlobalRotation = directionToTarget.Angle() + Calculate.Pi270;

		GlobalPosition = GlobalPosition.MoveToward(target2D.GlobalPosition, (float)delta * MyChaseSpeed);
	}

	private void DoEnterChaseRoutine(double delta)
	{
		mVisuals.TryPlayAnimation("chase");
	}

	private void OnCollectionRegionEnter(Area2D pArea)
	{
		// rare instance where player triggers collection region before gravitate region
		// gravitate region sets necessary info.
		if (mTargetExpGainHandler.IsNone)
		{
			OnChaseRegionEnter(pArea);
		}
		SafeGuard.Ensure(mTargetExpGainHandler.Available(out var handler));

		handler.SendExpReward(MyExperiencePoints);

		QueueFree();
	}

	private void OnChaseRegionEnter(Area2D pArea)
	{
		SafeGuard.Ensure(mTargetExpGainHandler.IsNone);

		var parent = pArea.GetParent();

		if (parent is IPickupCommandDispatcher ipcd)
		{
			mTargetExpGainHandler = Option<IPickupCommandDispatcher>.Ok(ipcd);
		}
		if (parent is LInfo2D lInfo2D)
		{
			mTargetInfo2D = Option<LInfo2D>.Ok(lInfo2D);
		}

		if (mTargetExpGainHandler.IsNone)
		{
			this.LogWarn("No handler for exp gain in hitbox owner.");
		}
		if (mTargetInfo2D.IsNone)
		{
			this.LogWarn("No info2D in hitbox owner.");
		}

		mGravitateListener.QueueFree();

		mState.GoTo(DoChaseRoutine);
	}


	[Export(PropertyHint.Range, "0,100")]
	uint MyExperiencePoints
	{
		get => myExperience;
		set
		{
			myExperience = value;
		}
	}

	[Export(PropertyHint.Range, "0,500")]
	float MyChaseSpeed { get; set; }

	[Export]
	bool ActivateAtStart { get; set; }

	private uint myExperience;

	AnimSoon mSpawnAnimAwaiter;
	LiteStates mState = new();

	Option<IPickupCommandDispatcher> mTargetExpGainHandler;
	Option<LInfo2D> mTargetInfo2D;
}
