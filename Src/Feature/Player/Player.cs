using Godot;
using static Godot.Mathf;
using static GodotStrict.Helpers.Logging.StrictLog;
using static GodotStrict.Helpers.Dependency.DependencyHelper;
using static GodotStrict.Types.Coroutine.AdventureExtensions;
using Adventure = System.Collections.Generic.IEnumerator<GodotStrict.Types.Coroutine.BaseSoon>;
using GodotStrict.Traits;
using GodotUtilities;
using GodotStrict.Helpers.Guard;


[GlobalClass]
// [UseAutowiring]
[Icon("res://GodotEditor/Icons/brain.png")]
public partial class Player : Node
{

	public override void _Ready()
	{
		// 依赖注入();
		SafeGuard.EnsureCanCast(Owner, out mUnitModel);
	}

	public override void _Process(double delta)
	{
		mUnitModel.MyPlannedMoveDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");
	}

	UnitModel mUnitModel;
}

