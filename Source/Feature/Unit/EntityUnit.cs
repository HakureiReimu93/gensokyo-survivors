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

[GlobalClass]
[UseAutowiring]
[Icon("res://GodotEditor/Icons/unit.png")]
public partial class EntityUnit : CharacterBody2D, LInfo2D, IFactionUnit
{
	[Autowired]
	IPilot mPilot;

	[Autowired]
	IValueBuf<Vector2> mMovementMware;


	public override void _Ready()
	{
		依赖注入();

		SafeGuard.Ensure(MyFaction != Faction.Inherit);
		SafeGuard.Ensure(MyMaxMovementSpeed != 0);
	}

	public override void _PhysicsProcess(double delta)
	{
		var movement = mPilot.CalculateMovement();
		mMovementMware.CalculateValue(ref movement);
		Velocity = movement * MyMaxMovementSpeed;

		MoveAndSlide();
	}

	public void SetFaction(Faction pFaction)
	{
		MyFaction = pFaction;

		FactionUtility.SetHardIdentityFor(this, pFaction);
		FactionUtility.SetHardListenerFor(this, pFaction);

		foreach (var child in GetChildren())
		{
			if (child is IFactionMember fm)
			{
				fm.MyFaction = pFaction;
			}
		}
	}
	public Faction MyFaction { get; private set; }

	[Export(PropertyHint.Range, "0,500")]
	public float MyMaxMovementSpeed { get; set; }
	public Node2D Entity => this;

}
