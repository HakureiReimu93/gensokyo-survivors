using Godot;
using GodotStrict.Helpers.Dependency;
using GensokyoSurvivors.Core.Interface;
using GodotStrict.Types;

[GlobalClass]
[Icon("res://Assets/Icons/unit.png")]
public partial class MobUnit : CharacterBody2D
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
}
