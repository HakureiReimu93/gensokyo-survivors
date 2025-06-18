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
public partial class Player : Node, IPilot
{
	[Autowired("@")]
	Option<HealthTrait> mHealth;

	public override void _Ready()
	{
		依赖注入();

		SafeGuard.EnsureCorrectType(Owner, out mUnit);
	}

	public Vector2 CalculateMoveDecision(double delta)
	{
		return Input.GetVector("move_left",
							   "move_right",
							   "move_up",
							   "move_down");
	}

	public Faction MyFaction { get; set; } = Faction.Ally;
	public EntityUnit Entity => mUnit;
	EntityUnit mUnit;
}
