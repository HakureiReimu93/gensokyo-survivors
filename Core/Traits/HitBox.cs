using GensokyoSurvivors.Core.Interface;
using GensokyoSurvivors.Core.Utility;
using Godot;
using GodotStrict.Helpers.Guard;


/// <summary>
/// Represents a hit box in the game, providing collision detection and damage handling.
/// 
/// Main features:
/// - Collision detection with mob units
/// - Damage calculation based on faction
/// - Automatic update of collision layer when faction changes
/// 
/// Autowired members:
/// - FactionEnum MyFaction (required)
/// </summary>
[GlobalClass]
[Icon("res://Assets//GodotEditor/Icons/hitbox.png")]
public partial class HitBox : Area2D, IFactionMember
{
	public override void _Ready()
	{
		SafeGuard.EnsureIsConstType<Node2D>(Owner);
		SafeGuard.Ensure(CollisionMask == 0, "Do not set the collision mask!");
		SafeGuard.Ensure(mFaction != FactionEnum.Undefined, "Set the faction");
	}

	public virtual void OnCollidedWith(HurtBox pHurtBox)
	{
		if (Owner is IPhysicalSkill skill &&
			pHurtBox.Owner is MobUnit mu)
		{
			skill.OnEnemyHit(mu);
		}
	}

	[Export]
	public FactionEnum MyFaction
	{
		get
		{
			return mFaction;
		}
		set
		{
			mFaction = value;
			CollisionLayer = FactionUtil.MaskFromFaction(mFaction);
		}
	}

	[Export]
	public float MyDamageOnHit { get; set; }

	FactionEnum mFaction;

}