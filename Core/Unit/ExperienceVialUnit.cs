using Godot;
using GodotStrict.Helpers;
using GodotStrict.Helpers.Guard;
using GodotUtilities;
using GensokyoSurvivors.Core.Utility;
using GodotStrict.Types;
using GensokyoSurvivors.Core.Interface.Lens;
using GodotStrict.Helpers.Logging;
using GodotStrict.Traits;

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

		SafeGuard.Ensure(MyChaseSpeed > 0);
		SafeGuard.Ensure(MyExperiencePoints > 0);

		mCollectListener.EnterBoxEntered += OnCollectionRegionEnter;
		mGravitateListener.EnterBoxEntered += OnGravitateRegionEnter;

		mVisuals.DoRegisterFallbackAnim("idle")
				.DoRegisterLoopingAnim("chase")
				.DoRegisterOneShotLockedAnim("spawn");

		mState.WithOwner(this)
				.PlanRoute(DoChaseRoutine, DoEnterChaseRoutine);

		mVisuals.TryPlayAnimationAndAwaitCompletion("spawn", out mSpawnAnimAwaiter);
		mSpawnAnimAwaiter.OnCompleted(DoLoadIdleStance);

		mCollectListener.SetEnabled(false);
		mGravitateListener.SetEnabled(false);

		SafeGuard.Ensure(mCollectListener.MyFaction == mGravitateListener.MyFaction);
	}

	public override void _Process(double delta)
	{
		mState.Process(delta);
	}

	private void DoLoadIdleStance()
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

		LookAt(target2D.GlobalPosition);
		GlobalPosition = GlobalPosition.MoveToward(target2D.GlobalPosition, (float)delta * MyChaseSpeed);
	}

	private void DoEnterChaseRoutine(double delta)
	{
		mVisuals.TryPlayAnimation("chase");
	}

	private void OnCollectionRegionEnter(HitBox pHitBox)
	{
		if (mTargetExpGainHandler.Unavailable(out var handler))
		{
			this.LogWarn("collection region entered, handler not found!");
		}

		handler.SendExpReward(MyExperiencePoints);

		QueueFree();
	}

	private void OnGravitateRegionEnter(HitBox pHitBox)
	{
		SafeGuard.Ensure(mTargetExpGainHandler.IsNone);
		mTargetExpGainHandler = pHitBox.DoGetContractInOwner<IPickupCommandDispatcher>();
		mTargetInfo2D = pHitBox.DoGetContractInOwner<LInfo2D>();

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
	int MyExperiencePoints
	{
		get => myExperience;
		set
		{
			SafeGuard.Ensure(value > 0);
			myExperience = value;
		}
	}

	[Export(PropertyHint.Range, "0,500")]
	float MyChaseSpeed { get; set; }

	private int myExperience;

	AnimSoon mSpawnAnimAwaiter;
	LiteStates mState;

	Option<IPickupCommandDispatcher> mTargetExpGainHandler;
	Option<LInfo2D> mTargetInfo2D;
}
