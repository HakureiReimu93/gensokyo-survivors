using Godot;
using GodotUtilities;
using GodotStrict.Helpers;

[GlobalClass]
[UseAutowiring]
[Icon("res://GodotEditor/Icons/script.png")]
public partial class GensokyoSurvivorsApp : Node
{
	public override void _Ready()
	{
		依赖注入();
		Prelude._Initialize(GetTree());
	}

	public override void _Process(double delta)
	{
		if (Input.IsKeyPressed(Key.Escape))
		{
			GetTree().Quit();
		}
		Prelude._Process(delta);
	}
}
