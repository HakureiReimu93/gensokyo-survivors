using Godot;
using GodotStrict.Helpers.Guard;
using GodotUtilities;
using GodotStrict.Traits;
using GensokyoSurvivors.Core.Presentation.UI.TimeDisplay;
using GodotStrict.Types;
using GodotStrict.Traits.EmptyImpl;

[GlobalClass]
[UseAutowiring]
[Icon("res://Assets/GodotEditor/Icons/manager.png")]
public partial class ArenaSession : Node, ILensProvider<LTimeLeftInfo>
{
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

	}

	class TimeLens(ArenaSession _en) : LTimeLeftInfo
	{
		Node ILens<Node>.Entity => _en;

		public Option<TimeBundle> GetTime()
		{
			return new TimeBundle(
				_en.mTimer.TimeLeft,
				_en.mTimer.WaitTime
			);
		}

	}
	public ArenaSession()
	{
		mLens = new(this);
	}
	private readonly TimeLens mLens;
	public LTimeLeftInfo Lens => mLens;

}
