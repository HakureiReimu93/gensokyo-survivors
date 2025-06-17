using Godot;
using GodotStrict.Helpers.Guard;
using GodotStrict.Traits;
using GodotUtilities;
using GensokyoSurvivors.Source.Library.Common;
using GensokyoSurvivors.Source.Library;
using GodotStrict.Types;

[GlobalClass]
[UseAutowiring]
[Icon("res://GodotEditor/Icons/brain.png")]
public partial class Player : Node, LInfo2D, IPilot
{
	[Autowired("@")]
	Option<HealthTrait> mHealth;

	public override void _Ready()
	{
		依赖注入();

		SafeGuard.EnsureCorrectType(Owner, out mUnit);
		mUnit.SetFaction(Faction.Ally);

		if (mHealth.IsSome)
			mHealth.Value.MyHealthChanged += ConsiderHealthChange;
	}

	public Vector2 CalculateMovement()
	{
		return Input.GetVector("move_left",
							   "move_right",
							   "move_up",
							   "move_down");
	}

	public void ConsiderHealthChange(float pPrev, float pCurrent, float pMax)
	{

	}

	public EntityUnit GetUnit()
	{
		return Owner as EntityUnit;
	}

	EntityUnit mUnit;
	public Node2D Entity => GetParent<Node2D>()!;
}
