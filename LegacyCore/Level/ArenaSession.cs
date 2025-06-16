using Godot;
using GodotStrict.Helpers.Guard;
using GodotUtilities;
using GodotStrict.Types;
using GensokyoSurvivors.Core.Interface.Lens;
using System;
using GodotStrict.AliasTypes;
using System.Collections.Generic;

[GlobalClass]
[UseAutowiring]
[Icon("res://Assets/GodotEditor/Icons/manager.png")]
public partial class ArenaSession : Node
{
	//TODO: implement channel scanner; change to channel scanner
	[Autowired("chan-session-time")]
	Scanner<LTimeLeftChannel> mTimeChannel;

	[Autowired("SessionDuration")]
	Timer mTimer;

	public override void _Ready()
	{
		__PerformDependencyInjection();

		GensokyoSurvivorsSession.Instance.MyMainSceneRoot = GetOwner<Node2D>();

		SafeGuard.Ensure(mTimer.WaitTime > 1f);
		SafeGuard.Ensure(mTimer.OneShot);
		SafeGuard.EnsureNotNull(MyVictoryDefeatUI);

		mTimer.Timeout += OnSessionTimeExpire;

		mSingleton = this;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		// may change later.
		if (mTimer.IsStopped() is false &&
			mTimeChannel.Available(out var chan))
		{
			// update those that are listening to changes to time.
			chan.ReceiveTime(new(mTimer.TimeLeft, mTimer.WaitTime));

			// consider updates to difficulty
			if (mDifficultyChangeTimer.Tick(delta))
			{
				mDifficultyChangeTimer.ResetWithCustomTime(mDifficultyChangeTimer.WaitTime * 1.5f);
				mDifficulty++;

				foreach (var subject in mDifficultyIncreaseSubjects)
				{
					if (subject.IsSome)
					{
						subject.Value.ConsiderNewDifficulty(mDifficulty);
					}
				}
				mDifficultyIncreaseSubjects.RemoveWhere(sub => sub.IsNone);
			}
		}
	}


	private void OnSessionTimeExpire()
	{
		if (SessionSignalBus.SingletonInstance.Unavailable(out var ssb)) return;

		// present the victory screen if the player still exists.
		if (GetTree().GetFirstNodeInGroup("id-player") is not null)
		{
			VictoryScreen ui = GensokyoSurvivorsSession.Instance.HostBlockingUIFromPacked<VictoryScreen>(
				MyVictoryDefeatUI,
				out Action unblock
			);

			ui.DescribeAsVictory();

			ui.ShowAndInvokeChosen(
				pQuit: () => GetTree().Quit(),
				pRestart: () => GetTree().ReloadCurrentScene(),
				unblock
			);

		}

		ssb.BroadcastSessionTimeExpired();
	}

	public void TriggerDefeat()
	{
		if (SessionSignalBus.SingletonInstance.Unavailable(out var ssb)) return;

		// present the victory screen if the player still exists.
		VictoryScreen ui = GensokyoSurvivorsSession.Instance.HostBlockingUIFromPacked<VictoryScreen>(
			MyVictoryDefeatUI,
			out Action unblock
		);

		ui.DescribeAsDefeat();

		ui.ShowAndInvokeChosen(
			pQuit: () => GetTree().Quit(),
			pRestart: () => GetTree().ReloadCurrentScene(),
			unblock
		);
	}

	public void AddDifficultyIncreaseRecipient(IDifficultyIncreasedSubject pSubject)
	{
		SafeGuard.EnsureNotNull(pSubject);
		SafeGuard.Ensure(mDifficultyIncreaseSubjects.Add(Option<IDifficultyIncreasedSubject>.Ok(pSubject)));
	}

	[Export]
	PackedScene MyVictoryDefeatUI;

	public static ArenaSession SingletonInstance => mSingleton;
	private static ArenaSession mSingleton;

	HashSet<Option<IDifficultyIncreasedSubject>> mDifficultyIncreaseSubjects = new();
	private LiteTimer mDifficultyChangeTimer = new(5f);
	private int mDifficulty = 0;
}

public interface IDifficultyIncreasedSubject
{
	public void ConsiderNewDifficulty(int pDifficulty);
}