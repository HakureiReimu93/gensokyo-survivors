using Godot;
using System;
using static Godot.Mathf;
using static GodotStrict.Helpers.Logging.StrictLog;
using static GodotStrict.Helpers.Dependency.DependencyHelper;
using GodotStrict.Helpers;
using GodotStrict.Helpers.Guard;
using GensokyoSurvivors.Core.Utility;

[GlobalClass]
public partial class HitBox : Area2D
{
	#region exports
	[Export]
	public FactionEnum MyFaction
	{
		get
		{
			return mFactionProp;
		}
		set
		{
			mFactionProp = value;
			CollisionMask = FactionUtil.LayerFromFaction(mFactionProp);
		}
	}
	FactionEnum mFactionProp;

	[Export]
	public float MyDamageOnHit { get; set; }

	#endregion

	public override void _Ready()
	{
		SafeGuard.EnsureIsConstType<CharacterBody2D>(Owner);
		SafeGuard.Ensure(CollisionMask == 0, "Do not set the collision mask!");
	}

	/// <summary>
	/// Only to be called from HurtBox.
	/// </summary>
	/// <param name="pHurtBox"></param>
	public void OnCollidedWith(HurtBox pHurtBox)
	{

	}
}
