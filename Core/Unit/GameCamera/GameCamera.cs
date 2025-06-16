using Godot;
using static GodotStrict.Helpers.Dependency.DependencyHelper;
using GodotStrict.Traits;

using GodotStrict.Types;
using GodotStrict.Helpers.Guard;
using GensokyoSurvivors.Core.Interface.Lens;

[GlobalClass]
public partial class GameCamera : Camera2D, LBoundsInfo2D
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
				1f - Mathf.Exp(-1f * (float)delta * 20f)
			);
		}
	}

	public Rect2 GetBounds()
	{
		var baseRect = GetViewportRect();
		baseRect.Position = GlobalPosition - (baseRect.Size / Zoom / 2);
		baseRect.Size /= Zoom;
		return baseRect;
	}

	Node2D ILens<Node2D>.Entity => this;

	[Export]
	string MyFocusID { get; set; } = "id-player";
	Scanner<LInfo2D> mTarget;

}