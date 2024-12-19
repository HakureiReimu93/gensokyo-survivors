using Godot;
using System;

public partial class GameCamera : Camera2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MakeCurrent();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var PlayerNodes = GetTree().GetNodesInGroup("player");
		
		if (PlayerNodes.Count > 0 )
		{
			var player = (Node2D)PlayerNodes[0];
			Offset = player.GlobalPosition;
		}
		
	}
}
