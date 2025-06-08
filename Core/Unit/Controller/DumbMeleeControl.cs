using System;
using GensokyoSurvivors.Core.Interface;
using Godot;
using GodotStrict.Helpers.Guard;
using GodotStrict.Traits;
using GodotStrict.Types;
using static GodotStrict.Helpers.Dependency.DependencyHelper;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/brain.png")]
public partial class DumbMeleeControl : Node, IMobUnitController
{
	[Signal]
	public delegate void OnRequestDieEventHandler();

	public override void _Ready()
	{
		SafeGuard.EnsureNotEqual(MyFocusID, "", "Cannot focus on nothing");
		SafeGuard.EnsureNotNull(Owner as MobUnit, "Owner must be a unit.");
		mOwner = Owner as Node2D;

		mTarget = ExpectFromGroup<LInfo2D>("id-player");

		mMotionStateMachine.PlanRoute(CalculateDefaultMotion)
						   .PlanRoute(CalculateSessionEndedMotion, IntoSessionEndedMotion)
						   .StartAt(CalculateDefaultMotion);
	}

	public override void _Process(double delta)
	{
		mCalculatedMovement = mMotionStateMachine.Calculate(delta);
	}

	private Vector2 CalculateDefaultMotion(double delta)
	{
		if (mTarget.Available(out var targetInfo))
		{
			return mOwner.GlobalPosition.DirectionTo(targetInfo.GlobalPosition);
		}
		else
		{
			return Vector2.Zero;
		}
	}

	private Vector2 CalculateSessionEndedMotion(double delta)
	{
		return default;
	}

	private Vector2 IntoSessionEndedMotion(double delta)
	{
		// also die, lol.
		EmitSignal(SignalName.OnRequestDie);

		return default;
	}

	public Vector2 GetNormalMovement() => mCalculatedMovement;

	public void OnControllerRequestDie(Action mEventHandler)
	{
		OnRequestDie += () => mEventHandler();
	}

	[Export]
	string MyFocusID { get; set; } = "id-player";

	Scanner<LInfo2D> mTarget;

	Node2D mOwner;
	LiteFunctionalStates<Vector2> mMotionStateMachine = new();
	Vector2 mCalculatedMovement;
}
