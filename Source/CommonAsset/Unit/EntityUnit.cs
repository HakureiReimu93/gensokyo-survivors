using Godot;
using System;
using static Godot.Mathf;
using static GodotStrict.Helpers.Logging.StrictLog;
using static GodotStrict.Helpers.Dependency.DependencyHelper;
using Adventure = System.Collections.Generic.IEnumerator<GodotStrict.Types.Coroutine.BaseSoon>;
using static GodotStrict.Types.Coroutine.AdventureExtensions;
using GodotUtilities;
using GodotStrict.Traits;
using GensokyoSurvivors.Source.Library;
using GodotStrict.Helpers.Guard;
using GensokyoSurvivors.Source.Library.Common;
using GodotStrict.Types;

[GlobalClass]
[UseAutowiring]
[Icon("res://GodotEditor/Icons/unit.png")]
public partial class EntityUnit : CharacterBody2D, LInfo2D
{
	[Autowired]
	IPilot mPilot;

	[Autowired]
	IValueMidWare<Vector2> mMovementMware;

	[Autowired]
	Option<HealthTrait> mHealth;

	[Autowired]
	OptionCollection<IVisualMidWare> mVisualMidWares;

	[Autowired("OnHurtMidWare")]
	Option<IVisualMidWare> mHurtMidWare;


	public override void _Ready()
	{
		依赖注入();

		MyFaction = mPilot.MyFaction;

		SafeGuard.Ensure(MyFaction != Faction.Inherit);
		SafeGuard.Ensure(MyMaxMovementSpeed != 0);

		if (mHealth.IsSome) mHealth.Value.MyDied += Die;
		if (mHealth.IsSome) mHealth.Value.MyHealthChanged += ConsiderDamageTaken;
	}

	public override void _PhysicsProcess(double delta)
	{
		var movement = mPilot.CalculateMoveDecision(delta);
		mMovementMware.CalculateValue(ref movement, delta);
		Velocity = movement * MyMaxMovementSpeed;

		if (mVisualMidWares.AnyAvailable(out var vMidWares))
		{
			foreach (var vMidWare in vMidWares)
			{
				vMidWare.NextEffect(this, delta);
			}
		}

		MoveAndSlide();
	}

	private void Die()
	{
		QueueFree();
	}

	private void ConsiderDamageTaken(float pOld, float pNew, float pMax)
	{
		if (mHurtMidWare.IsSome)
		{
			mHurtMidWare.Value.ActivateForSeconds(2f);
		}
	}

	public Faction MyFaction
	{
		get
		{
			return mFaction;
		}
		set
		{
			mFaction = value;

			FactionUtility.SetHardIdentityFor(this, value);
			FactionUtility.SetHardListenerFor(this, value);

			foreach (var child in GetChildren())
			{
				if (child is IFactionMember fm)
				{
					fm.MyFaction = value;
				}
			}
		}
	}

	private Faction mFaction;

	[Export(PropertyHint.Range, "0,500")]
	public float MyMaxMovementSpeed { get; set; }

	public Node2D Entity => this;

}
