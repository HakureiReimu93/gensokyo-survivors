using Godot;
using GodotStrict.Helpers.Dependency;
using GensokyoSurvivors.Core.Interface;
using GodotStrict.Types;
using GodotStrict.Traits;
using GodotStrict.Traits.EmptyImpl;
using static GodotStrict.Helpers.Logging.StrictLog;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/unit.png")]
public partial class MobUnit : CharacterBody2D, IKillable, ILensProvider<BaseImplInfo2D>
{
	[Export]
	public float MyMaxSpeed { get; private set; } = 200;

	// Required movement controller
	IMobUnitInput mMovementController;

	// Principal velocity buf (which is acceleration in this case.)
	Option<IVelocityBuf> mAccel;

	Option<HealthTrait> mHealth;
	Option<HurtBox> mHurtBox;

	bool mDead = false;
	public bool IsDead => mDead;

	public override void _Ready()
	{
		mMovementController = this.Require<IMobUnitInput>();

		mAccel = this.Optional<IVelocityBuf>();

		mHealth = this.Optional<HealthTrait>();
		mHurtBox = this.Optional<HurtBox>();

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
			finalVelocity = MyMaxSpeed * accel.GetVelocityBuf(moveDirection, MyMaxSpeed, delta);
		}

		Velocity = finalVelocity;

		MoveAndSlide();
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


	#region lens
	protected BaseImplInfo2D lens;
	public BaseImplInfo2D Lens => lens;


	public MobUnit() { lens = new(this); }
	#endregion

}
