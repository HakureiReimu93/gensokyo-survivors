using GensokyoSurvivors.Core.Interface;
using Godot;
using GodotStrict.Helpers.Guard;
using GodotStrict.Traits;
using GodotStrict.Types.Traits;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/script.png")]
public partial class UnitLayer : Node2D, LMother
{
	public override void _Ready()
	{
	}

	bool LMother.TryHost(Node child)
	{
		if (!SafeGuard.Ensure(child is not null) ||
			!SafeGuard.Ensure(child.GetParent() is null))
		{
			return false;
		}

		if (child is IDesignToken<Node> dt)
		{
			dt.Activate();
		}

		AddChild(child);
		return true;
	}

	Node ILens<Node>.Entity => this;
}
