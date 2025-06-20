using Godot;
using GodotStrict.Traits;
using GodotUtilities;
using GensokyoSurvivors.Src.Library;
using GodotStrict.Types;
using GodotStrict.Helpers.Guard;
using System.Net;
using GodotStrict.Helpers;

[GlobalClass]
[UseAutowiring]
[Icon("res://GodotEditor/Icons/brain.png")]
public partial class FairyController : Node, IUnitModelController
{
	[Autowired("id-player")]
	Scanner<LInfo2D> mPlayerRef;

	public override void _Ready()
	{
		依赖注入();
		SafeGuard.EnsureCanCastTo<UnitModel>(Owner, out mUnit);

		ProcessPriority = -1;
	}

	public override void _Process(double delta)
	{
		if (mPlayerRef.Available(out var player))
		{
			mPlannedMovement = mUnit.GlobalPosition.DirectionTo(player.GlobalPosition);
			// mPlannedMovement *= Calculate.RemapBounded(
			// 	mUnit.GlobalPosition.DistanceSquaredTo(player.GlobalPosition),
			// 	0,
			// 	200,
			// 	0.8f,
			// 	1f
			// );
		}
	}

	public void ConsiderDamageInfo(ref float pRaw, UnitBuf[] bufs)
	{
		// nothing for now.
	}

	public Vector2 MyPlannedMovement => mPlannedMovement;
	private Vector2 mPlannedMovement;
	private UnitModel mUnit;
}
