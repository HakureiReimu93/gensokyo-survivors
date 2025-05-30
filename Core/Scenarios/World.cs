using Godot;

public partial class World : Node2D
{
	public override void _Input(InputEvent @event)
	{
		if (Input.IsKeyPressed(Key.Escape))
		{
			GetTree().Quit();
		}
	}

	public override void _Ready()
	{
		GodotStrict.Helpers.Prelude._Initialize(GetTree());
	}

	public override void _Process(double delta)
	{
		GodotStrict.Helpers.Prelude._Process();
	}
}
