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
using GensokyoSurvivors.Core.Model;

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
		__PerformDependencyInjection();

		if (mHurtBox.Available(out var hurtBox))
		{
			hurtBox.MyTakeRawDamage += HandleHurtByDamageSource;
		}
		if (mHealth.Available(out var hp))
		{
			hp.MyHpDepleted += HandleHpDropToZero;
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
		finalVelocity *= MyBufs.ProductAll(buf => buf.MySpeedScale);

		Velocity = finalVelocity;

		MoveAndSlide();
	}

	private void HandleHurtByDamageSource(float pRawDamage)
	{
		if (mHealth.Available(out var hp))
		{
			hp.TriggerDamage(pRawDamage);
			// Add a hurt unit buff that lasts for a short period of time.
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
		QueueFree();
	}


	[Export]
	public float MyMaxSpeed { get; private set; } = 200;

	bool mDead = false;
	public bool IsDead => mDead;

	protected BaseImplInfo2D lens;
	public BaseImplInfo2D Lens => lens;
	public MobUnit() { lens = new(this); }

	public BufCollection MyBufs { get; protected set; } = new();

}
