using Godot;
using GodotStrict.Helpers.Guard;
using GodotUtilities;
using GodotStrict.Types;
using GensokyoSurvivors.Core.Interface.Lens;
using System;

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

	public static ArenaSession SingletonInstance => mSingleton;
	private static ArenaSession mSingleton;

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

			ui.ShowAndInvokeChosen(
				pQuit: () => GetTree().Quit(),
				pRestart: () => GetTree().ReloadCurrentScene(),
				unblock
			);
		}

		ssb.BroadcastSessionTimeExpired();
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
		}
	}

	[Export]
	PackedScene MyVictoryDefeatUI;
}
