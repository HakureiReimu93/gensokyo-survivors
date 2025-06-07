using Godot;
using GodotStrict.Helpers.Guard;
using GodotUtilities;
using GodotStrict.Types;
using GensokyoSurvivors.Core.Interface.Lens;
using GodotStrict.Types.PrevCurrent;

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

		SafeGuard.Ensure(mTimer.WaitTime > 1f);
		SafeGuard.Ensure(mTimer.OneShot);
		SafeGuard.Ensure(mTimer.Autostart);

		mTimer.Timeout += OnSessionTimeExpire;
	}

	private void OnSessionTimeExpire()
	{
		if (SessionSignalBus.SingletonInstance.Unavailable(out var ssb)) return;

		ssb.BroadcastSessionTimeExpired();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		// may change later.
		mSeconds.UpdateValue(Mathf.FloorToInt(mTimer.TimeLeft));
		if (mSeconds.CurrentLessThanPrevious() &&
			mTimeChannel.Available(out var chan))
		{
			// update those that are listening to changes to time.
			chan.ReceiveTime(new(mTimer.TimeLeft, mTimer.WaitTime));
		}
	}

	PrevCurrentValue<int> mSeconds;
}
