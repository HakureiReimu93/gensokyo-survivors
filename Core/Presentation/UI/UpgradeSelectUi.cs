using Godot;
using System;
using GodotStrict.Helpers.Guard;

using GodotUtilities;
using GodotStrict.Types.Coroutine;
using System.Collections.Generic;
using GodotStrict.Traits;
using Adventure = System.Collections.Generic.IEnumerator<GodotStrict.Types.Coroutine.BaseSoon>;
using System.Linq;
using static GodotStrict.Types.Coroutine.AdventureExtensions;

[GlobalClass]
[UseAutowiring]
[Icon("res://Assets/GodotEditor/Icons/script.png")]
public partial class UpgradeSelectUi : CanvasLayer, IUpgradeChoiceSelector
{
	[Autowired]
	AnimationPlayer mAnim;
	public override void _Ready()
	{
		__PerformDependencyInjection();
		// When creating the upgrade select ui, RESET track will make all cards fully visible
		Visible = false;

		SafeGuard.EnsureNotNull(MyLeftCardContainer);
		SafeGuard.EnsureNotNull(MyMiddleCardContainer);
		SafeGuard.EnsureNotNull(MyRightCardContainer);
		SafeGuard.EnsureCanInstantiate(MyCardUnitScene);

	}

	public FuncAdventureSoon<UpgradeMetaData> ShowAndAwaitChoice(UpgradeMetaData[] pUpgrades, Action unblockFunction)
	{
		SafeGuard.EnsureThatObject(pUpgrades.Length)
				.CanBe(1)
				.CanBe(2)
				.CanBe(3)
				.ButNothingElseBecause("UI does not support card count out of this range");

		for (int i = 0; i < pUpgrades.Length; i++)
		{
			var upgradeUi = MyCardUnitScene.Instantiate<UpgradeSelectCardUI>();
			SafeGuard.EnsureNotNull(upgradeUi);
			upgradeUi.Hydrate(pUpgrades[i]);

			mUpgradeUIs.Add(upgradeUi);
		}

		mUpgrades = pUpgrades;

		switch (pUpgrades.Length)
		{
			case 1:
				MyMiddleCardContainer.AddChild(mUpgradeUIs[0]);
				break;
			case 2:
				MyLeftCardContainer.AddChild(mUpgradeUIs[0]);

				MyRightCardContainer.AddChild(mUpgradeUIs[1]);
				break;
			case 3:
				MyLeftCardContainer.AddChild(mUpgradeUIs[0]);
				MyMiddleCardContainer.AddChild(mUpgradeUIs[1]);
				MyRightCardContainer.AddChild(mUpgradeUIs[2]);
				break;
			default:
				SafeGuard.Fail($"upgrade array out of range (length is {pUpgrades.Length})");
				throw new ArgumentOutOfRangeException(nameof(pUpgrades));
		}

		mUnblockFunction = unblockFunction;

		return this.StartFuncAdventure<UpgradeMetaData>(ShowSelectThenRewardUpgrade);
	}

	private Adventure ShowSelectThenRewardUpgrade()
	{
		Visible = true;
		SafeGuard.EnsureNotNull(mUpgrades);
		foreach (UpgradeSelectCardUI card in mUpgradeUIs) card.ConfirmButton.Disabled = true;

		yield return WaitTillAnimationFinish(mAnim, "fade-in");
		yield return WaitTillAnimationFinish(mAnim, $"fly-in_{mUpgradeUIs.Count}");
		foreach (UpgradeSelectCardUI card in mUpgradeUIs) card.ConfirmButton.Disabled = false;

		UnionSoon<int> cardButtonToIndex = WaitForChosenIndexed(
			mUpgradeUIs.Count,
			(idx) => new ButtonClickSoon(mUpgradeUIs[idx].ConfirmButton)
		);
		yield return cardButtonToIndex;

		foreach (UpgradeSelectCardUI card in mUpgradeUIs) card.ConfirmButton.Disabled = true;
		yield return WaitTillAnimationFinish(mAnim, $"fly-out_{cardButtonToIndex.Result + 1}");
		yield return WaitTillAnimationFinish(mAnim, "fade-out");

		// close the ui
		QueueFree();
		mUnblockFunction?.Invoke();

		yield return new AdventureResult<UpgradeMetaData>(
			mUpgrades[cardButtonToIndex.Result]
		);
	}

	[Export]
	MarginContainer MyLeftCardContainer { get; set; }

	[Export]
	MarginContainer MyMiddleCardContainer { get; set; }

	[Export]
	MarginContainer MyRightCardContainer { get; set; }

	[Export]
	PackedScene MyCardUnitScene { get; set; }

	CanvasLayer ILens<CanvasLayer>.Entity => this;

	List<UpgradeSelectCardUI> mUpgradeUIs = new();
	UpgradeMetaData[] mUpgrades;

	Action mUnblockFunction;
}

public interface IUpgradeChoiceSelector : ILens<CanvasLayer>
{
	public FuncAdventureSoon<UpgradeMetaData> ShowAndAwaitChoice(UpgradeMetaData[] choices, Action unblock);
}