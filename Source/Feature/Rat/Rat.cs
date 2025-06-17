using Godot;
using static Godot.Mathf;
using static GodotStrict.Helpers.Logging.StrictLog;
using static GodotStrict.Helpers.Dependency.DependencyHelper;
using static GodotStrict.Types.Coroutine.AdventureExtensions;
using Adventure = System.Collections.Generic.IEnumerator<GodotStrict.Types.Coroutine.BaseSoon>;
using GodotStrict.Traits;
using GodotUtilities;
using GodotStrict.Types;
using GensokyoSurvivors.Source.Library.Common;
using GodotStrict.Helpers.Guard;


[GlobalClass]
[UseAutowiring]
[Icon("res://GodotEditor/Icons/script.png")]
public partial class Rat : Node, IPilot
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

	public Vector2 CalculateMovement()
	{
		if (mPlayerRef.Available(out var player))
		{
			var proximityPenalty = Mathf.Remap(
				Mathf.Clamp(mUnit.GlobalPosition.DistanceTo(player.GlobalPosition), 0, 200),
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

	private void ConsiderHealthChange(float pPrevHp, float pCurrentHp, float pMaxHealth)
	{
		if (pCurrentHp / pMaxHealth < MyEnrageHpPercentThreshold)
		{
			mUnit.MyMaxMovementSpeed *= 1.5f;
		}
	}

	[Export]
	public float MyEnrageHpPercentThreshold { get; set; } = 0.5f;

	public EntityUnit GetUnit() => mUnit;
	private EntityUnit mUnit;

}

