using Godot;
using System;
using static Godot.Mathf;
using static GodotStrict.Helpers.Logging.StrictLog;
using static GodotStrict.Helpers.Dependency.DependencyHelper;
using GodotStrict.Helpers;
using GodotStrict.Helpers.Guard;
using GensokyoSurvivors.Core.Utility;

[GlobalClass]
[Icon("res://Assets//GodotEditor/Icons/hitbox.png")]
public partial class HitBox : Area2D
{
	#region exports
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
			CollisionMask = FactionUtil.MaskFromFaction(mFaction);
		}
	}
	FactionEnum mFaction;

	[Export]
	public float MyDamageOnHit { get; set; }

	#endregion

	public override void _Ready()
	{
		SafeGuard.EnsureIsConstType<Node2D>(Owner);
		SafeGuard.Ensure(CollisionLayer == 0, "Do not set the collision mask!");
		SafeGuard.Ensure(mFaction != FactionEnum.NONE, "Set the faction");
	}

	/// <summary>
	/// Only to be called from HurtBox.
	/// </summary>
	/// <param name="pHurtBox"></param>
	public void OnCollidedWith(HurtBox pHurtBox)
	{

	}
}
