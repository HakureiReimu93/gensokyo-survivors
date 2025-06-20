using Godot;
using static GodotStrict.Helpers.Logging.StrictLog;
using static GodotStrict.Helpers.Dependency.DependencyHelper;
using static GodotStrict.Types.Coroutine.AdventureExtensions;
using Adventure = System.Collections.Generic.IEnumerator<GodotStrict.Types.Coroutine.BaseSoon>;
using GodotStrict.Traits;
using GodotUtilities;
using GodotStrict.Helpers.Guard;
using GensokyoSurvivors.Src.Library;
using System;
using GodotStrict.Helpers;
using GodotStrict.Types;

[GlobalClass]
[UseAutowiring]
[Icon("res://GodotEditor/Icons/hurtbox.png")]
public partial class HurtBox : Area2D
{
	[Autowired]
	CollisionShape2D mShape;

	[Signal]
	public delegate void MyWasHurtEventHandler(float pDamageRaw, UnitBuf[] bufs);

	public override void _Ready()
	{
		依赖注入();
		SafeGuard.Ensure(MyFaction != Faction.None, "pick a faction for the hurtbox to target");

		CollisionLayer = 0;
		CollisionMask = MyFaction switch
		{
			Faction.None => 0b0000_0000,
			Faction.Ally => 0b0000_1000,
			Faction.Enemy => 0b0000_0100,
			Faction.Both => 0b0000_1000 | 0b0000_0100,
			_ => throw new System.NotImplementedException(),
		};

		AreaEntered += HandleAreaEnter;

		mDeferDisable = Callable.From(__DangerouslyDisableCollisionShape);
		mDeferEnable = Callable.From(__DangerouslyEnableCollisionShape);
	}

	private void HandleAreaEnter(Area2D area)
	{
		SafeGuard.EnsureCanCastTo<HitBox>(area, out var hbox);
		hbox.ConsiderHurtOtherHurtBox(this, out UnitBuf[] unitBufs);
		EmitSignal(SignalName.MyWasHurt, hbox.MyDamage, unitBufs);

		if (MyGracePeriod > 0)
		{
			mGracePeriodTimer.Restart();
			mDeferDisable.CallDeferred();
		}
	}


	public override void _Process(double delta)
	{
		if (mGracePeriodTimer.Tick(delta))
		{
			mDeferEnable.CallDeferred();
		}
	}

	[Export(PropertyHint.Range, "0,8")]
	float MyGracePeriod
	{
		get
		{
			return mGracePeriod;
		}
		set
		{
			mGracePeriod = value;

			mGracePeriodTimer.WaitTime = value;
			mGracePeriodTimer.TimeLeft = value;
		}
	}

	private float mGracePeriod;
	private LiteTimer mGracePeriodTimer;

	private Callable mDeferDisable;
	private Callable mDeferEnable;

	[Export]
	public Faction MyFaction { get; set; }

	private void __DangerouslyDisableCollisionShape() => mShape.SetDisabled(true);
	private void __DangerouslyEnableCollisionShape() => mShape.SetDisabled(false);
}
