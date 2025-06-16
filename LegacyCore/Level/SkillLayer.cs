using Godot;
using GodotStrict.Traits;
using GodotStrict.Types.Traits;

[GlobalClass]
public partial class SkillLayer : Node2D, LMother
{
	public override void _Ready()
	{
		base._Ready();
	}

	Node ILens<Node>.Entity => this;
}
