using Godot;
using static Godot.Mathf;
using static GodotStrict.Helpers.Logging.StrictLog;
using static GodotStrict.Helpers.Dependency.DependencyHelper;
using static GodotStrict.Types.Coroutine.AdventureExtensions;
using Adventure = System.Collections.Generic.IEnumerator<GodotStrict.Types.Coroutine.BaseSoon>;
using GodotStrict.Traits;
using GodotUtilities;
using GodotStrict.Helpers.Guard;
using GensokyoSurvivors.Src.Library;
using GodotStrict.Types;

[GlobalClass]
[UseAutowiring]
[Icon("res://GodotEditor/Icons/brain.png")]
public partial class Player : Node, IUnitModelController
{
	public override void _Ready()
	{
		依赖注入();
		SafeGuard.EnsureCanCastTo(Owner, out mUnitModel);
	}

	// Called by UnitModel
	// Health has
	public void ConsiderDamageInfo(ref float pDamageRaw, UnitBuf[] bufs)
	{
	}

	public override void _Process(double delta)
	{
		mPlannedMovement = Input.GetVector("move_left", "move_right", "move_up", "move_down");
	}

	UnitModel mUnitModel;
	public Vector2 MyPlannedMovement => mPlannedMovement;
	private Vector2 mPlannedMovement;
}

