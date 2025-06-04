using Godot;
using static GodotStrict.Helpers.Dependency.DependencyHelper;
using GodotStrict.Traits;

using GodotStrict.Types;
using GodotStrict.Helpers.Guard;
using GodotUtilities;

[GlobalClass]
public partial class GameCamera : Camera2D
{
	public override void _Ready()
	{
		SafeGuard.Ensure(IsCurrent());
		SafeGuard.EnsureNotEqual(MyFocusID, "", "Cannot focus on nothing");

		mTarget = ExpectFromGroup<LInfo2D>(MyFocusID);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (mTarget.JustAvailable(out var targetDataNew))
		{
			GlobalPosition = targetDataNew.GlobalPosition;
		}
		if (mTarget.Available(out var targetData))
		{
			GlobalPosition = GlobalPosition.Lerp(
				targetData.GlobalPosition,
				1f - Mathf.Exp(-1f * (float)delta * 10f)
			);
		}
	}

	[Export]
	string MyFocusID { get; set; } = "id-player";

	Scanner<LInfo2D> mTarget;
}