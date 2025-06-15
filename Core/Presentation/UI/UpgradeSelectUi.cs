using Godot;
using System;
using GodotStrict.Helpers.Guard;

using GodotUtilities;
using GodotStrict.Types.Coroutine;
using System.Collections.Generic;
using GodotStrict.Traits;
using Adventure = System.Collections.Generic.IEnumerator<GodotStrict.Types.Coroutine.BaseSoon>;
using System.Linq;

[GlobalClass]
[UseAutowiring]
[Icon("res://Assets/GodotEditor/Icons/script.png")]
public partial class UpgradeSelectUi : CanvasLayer, IUpgradeChoiceSelector
{
	[Autowired]
	AnimationPlayer mAnim;

	[Autowired("MainUI")]
	Control mMainUI;

	public override void _Ready()
	{
		__PerformDependencyInjection();
		// When creating the upgrade select ui, RESET track will make all cards fully visible
		Visible = false;
		mMainUI.Visible = false;

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
			var upgradeUi = MyCardUnitScene.Instantiate<UpgradeSelectUnitUI>();
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

	/// <summary>
	/// Yield return 
	/// </summary>
	/// <returns></returns>
	private Adventure ShowSelectThenRewardUpgrade()
	{
		SafeGuard.EnsureNotNull(mUpgrades);

		Visible = true;

		mAnim.Play("fade-in");
		yield return new AnimationCompletionSoon(mAnim, "fade-in");

		mMainUI.Visible = true;

		foreach (var card in mUpgradeUIs)
		{
			card.ConfirmButton.Disabled = true;
		}

		var flyInAnimName = $"fly-in_{mUpgradeUIs.Count}";

		mAnim.Play(flyInAnimName);
		yield return new AnimationCompletionSoon(mAnim, flyInAnimName);

		foreach (var card in mUpgradeUIs)
		{
			card.ConfirmButton.Disabled = false;
		}

		var buttons_3_toIdx = new UnionSoon<int>();

		for (int i = 0; i < mUpgradeUIs.Count; i++)
		{
			buttons_3_toIdx.AddSuspendedResult(
				new ButtonClickSoon(mUpgradeUIs[i].ConfirmButton),
				i
			);
		}

		yield return buttons_3_toIdx;

		foreach (var card in mUpgradeUIs)
		{
			card.ConfirmButton.Disabled = true;
		}

		var flyOutAnimName = $"fly-out_{buttons_3_toIdx.Result + 1}";
		mAnim.Play(flyOutAnimName);
		yield return new AnimationCompletionSoon(mAnim, flyOutAnimName);

		mAnim.Play("fade-out");
		yield return new AnimationCompletionSoon(mAnim, "fade-out");

		mMainUI.Visible = false;

		// close the ui
		QueueFree();
		mUnblockFunction?.Invoke();

		yield return new AdventureResult<UpgradeMetaData>(
			mUpgrades.ElementAt(buttons_3_toIdx.Result)
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

	List<UpgradeSelectUnitUI> mUpgradeUIs = new();
	UpgradeMetaData[] mUpgrades;

	Action mUnblockFunction;
}

public interface IUpgradeChoiceSelector : ILens<CanvasLayer>
{
	public FuncAdventureSoon<UpgradeMetaData> ShowAndAwaitChoice(UpgradeMetaData[] choices, Action unblock);
}