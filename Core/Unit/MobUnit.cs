using Godot;
using GensokyoSurvivors.Core.Interface;
using GodotStrict.Types;
using GodotStrict.Traits;
using GodotStrict.Traits.EmptyImpl;
using GodotUtilities;
using System.Collections.Generic;
using GodotStrict.AliasTypes;
using System;
using System.Linq;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/unit.png")]
[UseAutowiring]
public partial class MobUnit : CharacterBody2D, IKillable, ILensProvider<BaseImplInfo2D>
{
	// Required movement controller
	[Autowired]
	IMobUnitInput mMovementController;

	// Principal velocity buf (which is acceleration in this case.)
	[Autowired]
	Option<IScalarMiddleware<Vector2>> mAccel;

	[Autowired]
	Option<HealthTrait> mHealth;

	[Autowired]
	Option<HurtBox> mHurtBox;

	public override void _Ready()
	{
		if (mHurtBox.Available(out var hurtBox))
		{
			hurtBox.MyTakeRawDamage += HandleHurtByDamageSource;
		}
		if (mHealth.Available(out var hp))
		{
			hp.MyHpDepleted += HandleHpDepleted;
		}
	}

	public override void _Process(double delta)
	{
		if (mDead) return;

		var moveDirection = mMovementController.GetNormalMovement();
		var finalVelocity = moveDirection * MyMaxSpeed;

		if (mAccel.Available(out var accel))
		{
			finalVelocity *= accel.NextValue(moveDirection, delta);
		}

		// apply movement buf.
		finalVelocity *= GetTotalMovementBuf();

		Velocity = finalVelocity;

		MoveAndSlide();
	}

	public void AddUnitBuf(UnitBuf buf)
	{
		mBufs.Add(buf);
	}

	private float GetTotalMovementBuf()
	{
		return mBufs.Aggregate(
			1f,
			(currentMultiplier, currentBuf) => 
				currentMultiplier * currentBuf.MySpeedScale
		);
	}

	private void HandleHurtByDamageSource(float pRawDamage)
	{
		if (mHealth.Available(out var hp))
		{
			hp.TriggerDamage(pRawDamage);
		}
		else
		{
			TriggerDie();
		}
	}

	private void HandleHpDepleted(float pUnderflowRawDamage)
	{
		TriggerDie();
	}

	public void TriggerDie()
	{
		QueueFree();
	}


	[Export]
	public float MyMaxSpeed { get; private set; } = 200;

	List<UnitBuf> mBufs = [];
	bool mDead = false;
	public bool IsDead => mDead;

	protected BaseImplInfo2D lens;
	public BaseImplInfo2D Lens => lens;
	public MobUnit() { lens = new(this); }

	// Run dependency injection before _Ready() is called.
	public override void _Notification(int what) { if (what == NotificationSceneInstantiated) __PerformDependencyInjection(); }
}
