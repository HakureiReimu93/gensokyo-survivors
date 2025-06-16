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

	[Autowired("SkillRoster")]
	Node mSkillRoster;

	public override void _Ready()
	{
		__PerformDependencyInjection();

		SafeGuard.Ensure(Owner is MobUnit mu);
	}

	public void OnControllerRequestDie(Action pEventHandler)
	{
		RequestDie += () => pEventHandler();
	}

	public Vector2 GetNormalMovement() => Input.GetVector("move_left",
														  "move_right",
													  	  "move_up",
														  "move_down");

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
		if (pUpgrade.MyRewardScene != null &&
			!mUpgrades.TryGetUpgradeMetaDataFrom(pUpgrade.MyID, out _))
		{
			mSkillRoster.AddChild(pUpgrade.MyRewardScene.Instantiate());
		}

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
		if (ArenaSession.SingletonInstance is not null)
		{
			ArenaSession.SingletonInstance.TriggerDefeat();
		}
	}

	public static uint GetLevelUpExperienceRequirement(uint level)
	{
		return cBaseLevelUpExpRequirement + (level * 20);
	}

	[Signal]
	public delegate void RequestDieEventHandler();

	[Export]
	PackedScene MyUpgradeSelectorPacked { get; set; }

	public Node Entity => this;

	FuncAdventureSoon<UpgradeMetaData> mUpgradeAwaiter;
	Vector2 mCalculatedMovement;
	uint mCurrentExp = 0;
	uint mMaxXp = GetLevelUpExperienceRequirement(1);
	uint mLevel = 1;

	private const int cBaseLevelUpExpRequirement = (20);
}
