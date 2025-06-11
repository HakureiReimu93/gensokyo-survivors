using System;
using System.Security;
using GensokyoSurvivors.Core.Interface;
using GensokyoSurvivors.Core.Interface.Lens;
using Godot;
using GodotStrict.Helpers.Guard;
using GodotStrict.Types;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/brain.png")]
public partial class PlayerControl : Node, IMobUnitController, IPickupCommandReceiver
{
	[Signal]
	public delegate void RequestDieEventHandler();


	public override void _Ready()
	{
		SafeGuard.Ensure(Owner is MobUnit mu);

		if (SessionSignalBus.SingletonInstance.Available(out var ssb))
		{
			ssb.SessionTimeExpired += SessionTimeExpired;
		}

		mStateMachine.WithOwner(this)
					 .PlanRoute(CalculateDefaultMotion)
					 .PlanRoute(CalculateSessionEndedMotion, IntoSessionEndedMotion)
					 .StartAt(CalculateDefaultMotion);
	}

	public override void _Process(double delta)
	{
		mCalculatedMovement = mStateMachine.Calculate(delta);
	}

	private Vector2 CalculateDefaultMotion(double delta)
	{
		return Input.GetVector("move_left",
						"move_right",
						"move_up",
						"move_down");
	}

	private Vector2 CalculateSessionEndedMotion(double delta)
	{
		return Vector2.Zero;
	}


	private Vector2 IntoSessionEndedMotion(double delta)
	{
		return default;
	}

	private void SessionTimeExpired()
	{
		mStateMachine.GoTo(CalculateSessionEndedMotion);
	}

	public void OnControllerRequestDie(Action pEventHandler)
	{
		RequestDie += () => pEventHandler();
	}

	public Vector2 GetNormalMovement() => mCalculatedMovement;

	public void ReceiveExpReward(int pExperienceGainedRaw)
	{
		mTotalExperience += pExperienceGainedRaw;
	}

	public Node Entity => this;

	Vector2 mCalculatedMovement;
	LiteFunctionalStates<Vector2> mStateMachine = new();

	int mTotalExperience;
}
