using Godot;
using GodotStrict.Traits;
using GodotUtilities;
using GodotStrict.Types;
using GensokyoSurvivors.Source.Library.Common;
using GodotStrict.Helpers.Guard;
using GensokyoSurvivors.Source.Library;
using GodotStrict.Helpers;


[GlobalClass]
[UseAutowiring]
[Icon("res://GodotEditor/Icons/script.png")]
public partial class Fairy : Node, IPilot
{
	[Autowired("id-player")]
	Scanner<LInfo2D> mPlayerRef;

	[Autowired("@")]
	Option<HealthTrait> mHealth;

	public override void _Ready()
	{
		依赖注入();
		SafeGuard.EnsureCorrectType(Owner, out mUnit);
		if (mHealth.IsSome) mHealth.Value.MyHealthChanged += ConsiderHealthChange;
	}

	public Vector2 CalculateMoveDecision(double delta)
	{
		if (mPlayerRef.Available(out var player))
		{
			var proximityPenalty = Calculate.RemapBounded(
				mUnit.GlobalPosition.DistanceTo(player.GlobalPosition),
				200,
				0,
				
				1,
				0.5f
			);
			return mUnit.GlobalPosition.DirectionTo(player.GlobalPosition) * proximityPenalty;
		}
		else
		{
			return Vector2.Zero;
		}
	}

	public void ConsiderHealthChange(float pPrevHp, float pCurrentHp, float pMaxHealth)
	{
		if (pCurrentHp / pMaxHealth < MyEnrageHpPercentThreshold)
		{
			mUnit.MyMaxMovementSpeed *= 1.5f;
		}
	}

	[Export]
	public float MyEnrageHpPercentThreshold { get; set; } = 0.5f;

	public Faction MyFaction => Faction.Enemy;

	public EntityUnit Entity => mUnit;
	private EntityUnit mUnit;

}

