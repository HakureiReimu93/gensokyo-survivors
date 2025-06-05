using Godot;
using static GodotStrict.Helpers.Dependency.DependencyHelper;
using GodotStrict.Traits;

using GodotStrict.Types;
using GodotStrict.Helpers.Guard;
using GodotUtilities;
using GensokyoSurvivors.Core.Interface.Lens;
using GodotStrict.Traits.EmptyImpl;

[GlobalClass]
public partial class GameCamera : Camera2D, ILensProvider<LBoundsInfo2D>
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

	public LBoundsInfo2D Lens => lens;
	private readonly BoundsInfo2D lens;
	Scanner<LInfo2D> mTarget;

	public class BoundsInfo2D : BaseImpl<Camera2D>, LBoundsInfo2D
	{
		public BoundsInfo2D(Camera2D _en) : base(_en)
		{
		}

		Node2D ILens<Node2D>.Entity => en;

		public Rect2 GetBounds()
		{
			return en.GetViewportRect();
		}
	}

	public GameCamera()
	{
		lens = new(this);
	}
}