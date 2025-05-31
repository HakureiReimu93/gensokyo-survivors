using Godot;
using GodotStrict.Helpers.Dependency;
using GensokyoSurvivors.Core.Interface;
using GodotStrict.Types;
using GodotStrict.Traits;
using GodotStrict.Traits.EmptyImpl;

[GlobalClass]
[Icon("res://Assets/Icons/unit.png")]
public partial class MobUnit : CharacterBody2D, ILensProvider<MobUnit.Impl>
{

	const float cFallbackSpeed = 200;

	IMobUnitInput mMovementController;
	Option<IVelocityMiddleware> mMoveMiddleware;

	public override void _Ready()
	{
		mMovementController = this.Require<IMobUnitInput>();
		mMoveMiddleware = this.Optional<IVelocityMiddleware>();
		if (!mMoveMiddleware)
		{
			GodotStrict.Helpers.Logging.StrictLog.LogWarn("Not using middleware; using fallback movement.");
		}
	}

	public override void _Process(double delta)
	{
		var moveDirection = mMovementController.GetNormalMovement();
		if (mMoveMiddleware.Available(out var moveMiddleware))
		{
			Velocity = moveMiddleware.GetNextVelocity(moveDirection, delta);
		}
		else
		{
			Velocity = moveDirection * cFallbackSpeed;
		}
		MoveAndSlide();
	}

	#region lens
	protected Impl impl;
	Impl ILensProvider<Impl>.Lens => impl;
	public class Impl : BaseImplInfo2D
	{
		public Impl(MobUnit _en) : base (_en) {}
	}
	public MobUnit()
	{
		impl = new(this);
	}
	#endregion
}
