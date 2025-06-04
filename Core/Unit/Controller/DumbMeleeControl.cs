using GensokyoSurvivors.Core.Interface;
using Godot;
using GodotStrict.Helpers.Guard;
using GodotStrict.Traits;
using GodotStrict.Types;
using static GodotStrict.Helpers.Dependency.DependencyHelper;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/brain.png")]
public partial class DumbMeleeControl : Node, IMobUnitInput
{
	public override void _Ready()
	{
		SafeGuard.EnsureNotEqual(MyFocusID, "", "Cannot focus on nothing");
		SafeGuard.EnsureNotNull(Owner as MobUnit, "Owner must be a unit.");
		mOwner = Owner as Node2D;

		mTarget = ExpectFromGroup<LInfo2D>("id-player");
	}

	public Vector2 GetNormalMovement()
	{
		if (mTarget.Available(out var targetInfo))
		{
			return mOwner.GlobalPosition.DirectionTo(targetInfo.GlobalPosition);
		}
		else
		{
			return Vector2.Zero;
		}
	}

	[Export]
	string MyFocusID { get; set; } = "id-player";

	Scanner<LInfo2D> mTarget;

	Node2D mOwner;
}
