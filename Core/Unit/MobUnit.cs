using Godot;
using GensokyoSurvivors.Core.Interface;
using GodotStrict.Types;
using GodotStrict.Traits;
using GodotStrict.Traits.EmptyImpl;
using GodotUtilities;
using GensokyoSurvivors.Core.Model;
using GodotStrict.Helpers.Guard;

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

		SafeGuard.EnsureNotNull(MyTakeDamageBufScene);

		if (mHurtBox.Available(out var hurtBox))
		{
			hurtBox.MyTakeRawDamage += HandleHurtByDamageSource;
		}
		if (mHealth.Available(out var hp))
		{
			hp.MyHpDepleted += HandleHpDropToZero;
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

		// apply color buf
		Modulate = MyBufs.ColorMultiplyAll();

		MoveAndSlide();
	}

	private void HandleHurtByDamageSource(float pRawDamage)
	{
		if (mHealth.Available(out var hp))
		{
			hp.TriggerDamage(pRawDamage);

			var takeDamageBuf = MyTakeDamageBufScene.InstantiateOrNull<TakeDamageBuf>();
			SafeGuard.EnsureNotNull(takeDamageBuf);

			AddUnitBuf(takeDamageBuf);

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

	protected BufCollection MyBufs { get; set; } = new();

	[Export]
	PackedScene MyTakeDamageBufScene;

}
