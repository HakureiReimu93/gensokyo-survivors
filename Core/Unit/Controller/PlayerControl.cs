using System;
using GensokyoSurvivors.Core.Interface;
using GensokyoSurvivors.Core.Interface.Lens;
using GensokyoSurvivors.Core.Interface.Lens.Experience;
using GensokyoSurvivors.Core.Model;
using Godot;
using GodotStrict.Helpers;
using GodotStrict.Helpers.Guard;
using GodotStrict.Types;
using GodotStrict.Types.Coroutine;
using GodotUtilities;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/brain.png")]
[UseAutowiring]
public partial class PlayerControl : Node, IMobUnitController, IPickupCommandReceiver
{
	[Autowired("chan-experience")]
	Scanner<LExperienceChannel> mExperienceListeners;

	[Autowired]
	UpgradeLayer mUpgrades;

	public override void _Ready()
	{
		__PerformDependencyInjection();

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

	public void ReceiveExpReward(uint pExperienceGainedRaw)
	{
		mCurrentExp += pExperienceGainedRaw;
		if (mCurrentExp >= mMaxXp)
		{
			// will populate mLevel and mCurrentExp when called.
			UpdateLevelAndExp(mCurrentExp, mLevel, out mLevel, out mCurrentExp);
			mMaxXp = GetLevelUpExperienceRequirement(mLevel);

			// TODO: handle multi-level up
			var possibleUpgrades = mUpgrades.DoGetNextUpgrades_3();

			if (IsInstanceValid(MyUpgradeSelectorPacked))
			{
				var ui = GensokyoSurvivorsSession.Instance.HostBlockingUIFromPacked<UpgradeSelectUi>(
					MyUpgradeSelectorPacked,
					out var unblockFunction
				);

				ui.ShowAndAwaitChoice(possibleUpgrades, unblockFunction)
					.OnCompletedWithOutput(DoAddNewUpgrade);
			}
			else
			{
				DoAddNewUpgrade(
					Calculate.RandomCollectionItem(possibleUpgrades)
				);
			}
		}
		if (mExperienceListeners.Available(out var expListeners))
		{
			expListeners.ReceiveExperience(new ExperienceBundle(
				mCurrentExp,
				mMaxXp,
				mLevel
			));
		}
	}

	private void DoAddNewUpgrade(UpgradeMetaData pUpgrade)
	{
		mUpgrades.HostUpgrade(pUpgrade);
	}

	private static void UpdateLevelAndExp(uint currentExperience, uint currentLevel, out uint newLevel, out uint expRemainder)
	{
		var nextLevelRequirement = GetLevelUpExperienceRequirement(currentLevel);
		while (nextLevelRequirement <= currentExperience)
		{
			currentLevel += 1;
			currentExperience -= nextLevelRequirement;
		}

		newLevel = currentLevel;
		expRemainder = currentExperience;
	}

	public void OnUnitDie()
	{
		// upload final experience to PostmortemSession 
	}

	public static uint GetLevelUpExperienceRequirement(uint level)
	{
		return cBaseLevelUpExpRequirement + (level * 20);
	}

	[Signal]
	public delegate void RequestDieEventHandler();

	[Export]
	PackedScene MyUpgradeSelectorPacked { get; set; }

	private const int cBaseLevelUpExpRequirement = (20);

	public Node Entity => this;

	Vector2 mCalculatedMovement;
	LiteFunctionalStates<Vector2> mStateMachine = new();

	FuncAdventureSoon<UpgradeMetaData> mUpgradeAwaiter;

	uint mCurrentExp = 0;
	uint mMaxXp = GetLevelUpExperienceRequirement(1);
	uint mLevel = 1;
}
