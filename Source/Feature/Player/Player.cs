using Godot;
using GodotStrict.Helpers.Guard;
using GodotStrict.Traits;
using GodotUtilities;
using GensokyoSurvivors.Source.Library.Common;
using GensokyoSurvivors.Source.Library;

[GlobalClass]
[UseAutowiring]
[Icon("res://GodotEditor/Icons/script.png")]
public partial class Player : Node, LInfo2D, IPilot
{
	public override void _Ready()
	{
		__PerformDependencyInjection();

		SafeGuard.EnsureCorrectType(Owner, out mUnit);
		mUnit.SetFaction(Faction.Ally);
	}

	public Vector2 CalculateMovement()
	{
		return Input.GetVector("move_left",
							   "move_right",
							   "move_up",
							   "move_down");
	}

	public EntityUnit GetUnit()
	{
		return Owner as EntityUnit;
	}

	EntityUnit mUnit;
	public Node2D Entity => GetParent<Node2D>()!;
}
