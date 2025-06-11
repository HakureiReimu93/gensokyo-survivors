using Godot;
using GensokyoSurvivors.Core.Interface;
using GodotStrict.Types;
using GodotUtilities;
using GensokyoSurvivors.Core.Model;
using GodotStrict.Helpers.Guard;
using GensokyoSurvivors.Core.Utility;
using GodotStrict.Types.Locked;
using GensokyoSurvivors.Core.Interface.Lens;
using GodotStrict.Traits;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/unit.png")]
[UseAutowiring]
public partial class MobUnit : CharacterBody2D,
	IKillable,
	IPickupCommandDispatcher,
	LInfo2D
{
	[Autowired]
	UnitMonoVisual mVisuals;

	[Autowired]
	TakeDamageBuf mOnTakeDamageBufTemplate;

	// Required movement controller
	[Autowired]
	IMobUnitController mMovementController;
	// Principal velocity buf (which is acceleration in this case.)
	[Autowired]
	Option<IScalarMiddleware<Vector2>> mAccel;

	[Autowired]
	Option<HealthTrait> mHealth;

	[Autowired]
	Option<HurtBox> mHurtBox;

	public override void _Ready()
	{
		__PerformDependencyInjection();

		if (mHurtBox.Available(out var hurtBox))
		{
			hurtBox.MyTakeRawDamage += HandleHurtByDamageSource;
		}
		if (mHealth.Available(out var hp))
		{
			hp.MyHpDepleted += HandleHpDropToZero;
		}

		mMovementController.OnControllerRequestDie(TriggerDie);

		mVisuals.DoRegisterFallbackAnim("idle")
				.DoRegisterLoopingAnim("walk")
				.DoRegisterFinalAnim("die");

	}

	private void SwitchToSessionOver(double delta)
	{
		// check faction. if enemy, just die.
		if (!mHurtBox.Available(out var hb) &&
			hb.MyFaction == FactionEnum.Enemy)
		{
			TriggerDie();
		}
	}

	public void AddUnitBuf(UnitBuf ub)
	{
		MyBufs.Add(ub);
		ub.OnUnitAddsMe(this);
	}

	public void RemoveUnitBuf(UnitBuf ub)
	{
		MyBufs.RemoveSpecificUnitBuf(ub);
		ub.OnUnitRemovesMe();
	}

	public override void _Process(double delta)
	{
		if (mDead) return;

		// apply buf processing
		MyBufs.ProcessAll(delta);

		var moveDirection = mMovementController.GetNormalMovement();
		var finalVelocity = moveDirection * MyMaxSpeed;

		if (mAccel.Available(out var accel))
		{
			finalVelocity *= accel.NextValue(moveDirection, delta);
		}

		// apply movement buf.
		finalVelocity *= MyBufs.ProductAll(buf => buf.MyBaseMovementSpeedScale);

		Velocity = finalVelocity;

		// Talk to unit anim
		if (moveDirection.IsZeroApprox() || finalVelocity.IsZeroApprox())
		{
		}
		else
		{
		}

		// apply color buf
		Modulate = Colors.White * MyBufs.ColorMultiplyAll();

		MoveAndSlide();
	}

	private void HandleHurtByDamageSource(float pRawDamage)
	{
		if (mHealth.Available(out var hp))
		{
			hp.TriggerDamage(pRawDamage);

			Node duplicated = mOnTakeDamageBufTemplate.Duplicate();
			SafeGuard.Ensure(duplicated is TakeDamageBuf);
			TakeDamageBuf buf = duplicated as TakeDamageBuf;

			AddUnitBuf(buf);
		}
		else
		{
			TriggerDie();
		}
	}

	private void HandleHpDropToZero(float pUnderflowRawDamage)
	{
		TriggerDie();
	}

	public void TriggerDie()
	{
		SafeGuard.Ensure(mDead.Never());
		SafeGuard.Ensure(mDeathAnimCompetionStatus.IsLocked is false);

		if (mVisuals.TryPlayAnimationAndAwaitCompletion("die", out var soon) is Outcome.Succeed)
		{
			mDeathAnimCompetionStatus.LockTo(soon);
			soon.OnCompleted(QueueFree);
		}
	}

	public void TriggerDieForcely()
	{
		TriggerDie();
	}

	// Chain of responsibility that should send Exp to the player controller.
	public Outcome SendExpReward(int pExperienceGainedRaw)
	{
		if (mMovementController is not IPickupCommandReceiver pcr)
		{
			return Outcome.NoHandler;
		}

		pcr.ReceiveExpReward(pExperienceGainedRaw);
		return 0;
	}

	[Export]
	public float MyMaxSpeed { get; private set; } = 200;

	Lockable<AnimSoon> mDeathAnimCompetionStatus;
	TriggerFlag mDead;
	public bool IsDead => mDead;
	protected BufCollection MyBufs { get; set; } = new();

	Node2D ILens<Node2D>.Entity => this;
}


