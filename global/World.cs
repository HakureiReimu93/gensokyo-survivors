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
}
