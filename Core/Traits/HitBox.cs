using Godot;
using GodotStrict.Helpers.Guard;
using GensokyoSurvivors.Core.Utility;
using GensokyoSurvivors.Core.Interface;

[GlobalClass]
[Icon("res://Assets//GodotEditor/Icons/hitbox.png")]
public partial class HitBox : Area2D
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
