using Godot;
using GodotStrict.Helpers.Dependency;
using GensokyoSurvivors.Core.Interface;
using GodotStrict.Types;
using GodotStrict.Traits;
using GodotStrict.Traits.EmptyImpl;
using static GodotStrict.Helpers.Logging.StrictLog;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/unit.png")]
public partial class MobUnit : CharacterBody2D, ILensProvider<BaseImplInfo2D>
{
	[Export]
	public float MyMaxSpeed { get; private set; } = 200;

	IMobUnitInput mMovementController;

	// Principal velocity buf (which is acceleration in this case.)
	Option<IVelocityBuf> mAccel;

	public override void _Ready()
	{
		mMovementController = this.Require<IMobUnitInput>();
		mAccel = this.Optional<IVelocityBuf>();
		if (!mAccel)
		{
			LogWarn("Expected acceleration component, none found.");
		}
	}

	public override void _Process(double delta)
	{
		var moveDirection = mMovementController.GetNormalMovement();
		var finalVelocity = moveDirection * MyMaxSpeed;

		if (mAccel.Available(out var accel))
		{
			finalVelocity *= accel.GetVelocityBuf(moveDirection, MyMaxSpeed, delta);
		}

		Velocity = finalVelocity;

		MoveAndSlide();
	}

	#region lens
	protected BaseImplInfo2D lens;
	public BaseImplInfo2D Lens => lens;
	public MobUnit() { lens = new(this); }
	#endregion

}
