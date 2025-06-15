using Godot;
using GensokyoSurvivors.Core.Interface;
using GodotStrict.Types;
using GodotUtilities;
using GensokyoSurvivors.Core.Model;
using GensokyoSurvivors.Core.Utility;
using GensokyoSurvivors.Core.Interface.Lens;
using GodotStrict.Helpers.Guard;
using GodotStrict.Types.Locked;
using GodotStrict.Traits;
using GodotStrict.Types.Traits;
using GodotStrict.Helpers.Logging;
using GodotStrict.Helpers;
using System;
using System.Linq;
using GodotStrict.Types.Coroutine;
using System.Collections.ObjectModel;
using GodotStrict.AliasTypes;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/unit.png")]
[UseAutowiring]
public partial class MobUnit : CharacterBody2D,
	IKillable,
	IPickupCommandDispatcher,
	LInfo2D,
	INewUpgradeDispatcher
{
	[Autowired]
	UnitMonoVisual mVisuals;

	[Autowired]
	TakeDamageBuf mOnTakeDamageBufTemplate;

	[Autowired]
	Option<IMobUnitController> mUnitBrain;
	// Principal velocity buf (which is acceleration in this case.)
	[Autowired]
	Option<IScalarMiddleware<Vector2>> mAccel;

	[Autowired]
	Option<HealthTrait> mHealth;

	[Autowired]
	Option<HurtBox> mHurtBox;

	[Autowired("id-unit-layer")]
	Scanner<LMother> mUnitLayerRef;

	[Autowired("ReleaseOnDieLoot")]
	Option<Node2D> mDropOnDeath;

	[Autowired]
	Option<Node> mSkillRoster;

	[Autowired]
	Option<HealthBar> mHealthBar;

	[Signal]
	public delegate void OnUnitDieEventHandler();

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

		if (mUnitBrain.Available(out var controller))
		{
			controller.OnControllerRequestDie(TriggerDie);
		}

		if (mHealthBar.Available(out var hpBar))
		{
			hpBar.UpdatePercentage(new normal(1f));
		}

		mVisuals.DoRegisterFallbackAnim("idle")
				.DoRegisterLoopingAnim("walk")
				.DoRegisterFinalAnim("die")
				.TryPlayAnimation("idle");
	}

	public void OnDie(Action pContinuation)
	{
		OnUnitDie += () => pContinuation();
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

		// Maybe this can fix leaked ObjectDB instances?
		ub.QueueFree();
	}

	public override void _Process(double delta)
	{
		if (mDead) return;

		// apply buf processing
		MyBufs.ProcessAll(delta);

		var moveDirection = mUnitBrain.MatchValue(
			some: (val) => val.GetNormalMovement(),
			none: () => Vector2.Zero
		);

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
			mVisuals.TryPlayAnimation("idle");
		}
		else
		{
			mVisuals.TryPlayAnimation("walk");
		}

		// apply color buf
		Modulate = Colors.White * MyBufs.ColorMultiplyAll();

		MoveAndSlide();
	}

	private void HandleHurtByDamageSource(float pRawDamage)
	{
		if (mHealth.Available(out var hp) && !mDead)
		{
			hp.TriggerDamage(pRawDamage);

			Node duplicated = mOnTakeDamageBufTemplate.Duplicate();
			SafeGuard.Ensure(duplicated is TakeDamageBuf);
			TakeDamageBuf buf = duplicated as TakeDamageBuf;

			if (hp.GetHealth().IsNegative() is false)
			{
				AddUnitBuf(buf);
			}

			if (mHealthBar.Available(out var healthBar))
			{
				healthBar.UpdatePercentage(
					new normal(hp.GetHealth() / hp.GetMaxHealth())
				);
			}
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

		EmitSignal(SignalName.OnUnitDie);

		MyBufs.OnDieAll();

		if (mVisuals.TryPlayAnimationAndAwaitCompletion("die", out var soon) is Outcome.Succeed)
		{
			mDeathAnimCompetionStatus.LockTo(soon);
			soon.OnCompleted(_Die);
		}
	}

	/// <summary>
	/// Final dead effects
	/// </summary>
	protected void _Die()
	{
		if (mUnitLayerRef.Available(out var unitLayer) &&
			mDropOnDeath.Available(out var drops))
		{
			foreach (var dropsChild in drops.GetChildren().Cast<Node2D>())
			{
				drops.RemoveChild(dropsChild);
				unitLayer.TryHost(dropsChild);
				dropsChild.GlobalPosition = GlobalPosition;
			}
		}
		else
		{
			this.LogWarn("Could not find unitlayer");
		}

		if (mUnitBrain.Available(out var controller))
		{
			controller.OnUnitDie();
		}

		QueueFree();
	}

	public void TriggerDieForcely()
	{
		TriggerDie();
	}

	// Chain of responsibility that should send Exp to the player controller.
	public Outcome SendExpReward(uint pExperienceGainedRaw)
	{
		if (mUnitBrain.IsSome &&
			mUnitBrain.Value is not IPickupCommandReceiver pcr)
		{
			return Outcome.NoHandler;
		}
		else
		{
			var reciever = mUnitBrain.Value as IPickupCommandReceiver;
			reciever.ReceiveExpReward(pExperienceGainedRaw);
		}

		return 0;
	}

	public void SendNewUpgrade(UpgradeMetaData pUpgrade, ReadOnlyDictionary<UpgradeMetaData, uint> pAllUpgrades)
	{
		SafeGuard.Ensure(mSkillRoster.IsSome);

		var subjects = mSkillRoster.Value
							.GetChildren()
							.Cast<INewUpgradeSubject>();

		foreach (var subject in subjects)
		{
			subject.ConsiderNewUpgrade(pUpgrade, pAllUpgrades);
		}
	}

	[Export]
	public float MyMaxSpeed { get; private set; } = 200;

	Lockable<AnimationCompletionSoon> mDeathAnimCompetionStatus;
	TriggerFlag mDead;
	public bool IsDead => mDead;
	protected BufCollection MyBufs { get; set; } = new();

	Node2D ILens<Node2D>.Entity => this;
}


