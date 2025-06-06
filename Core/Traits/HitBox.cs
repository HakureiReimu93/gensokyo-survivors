using Godot;
using GodotStrict.Helpers.Guard;
using GensokyoSurvivors.Core.Utility;

[GlobalClass]
[Icon("res://Assets//GodotEditor/Icons/hitbox.png")]
public partial class HitBox : Area2D
{
	public override void _Ready()
	{
		SafeGuard.EnsureIsConstType<Node2D>(Owner);
		SafeGuard.Ensure(CollisionMask == 0, "Do not set the collision mask!");
		SafeGuard.Ensure(mFaction != FactionEnum.NONE, "Set the faction");
	}

	public virtual void OnCollidedWith(HurtBox pHurtBox)
	{
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
