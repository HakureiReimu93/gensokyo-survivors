using GensokyoSurvivors.Core.Interface;
using Godot;
using GodotStrict.Traits;
using GodotStrict.Types.Traits;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/script.png")]
public partial class UnitLayer : Node2D, LMother
{
	public override void _Ready()
	{
		base._Ready();
		ChildEnteredTree += OnAddChild;
	}

	private void OnAddChild(Node child)
	{
		if (child is IDesignToken<Node> dt)
		{
			dt.Activate();
		}
	}
	Node ILens<Node>.Entity => this;
}
