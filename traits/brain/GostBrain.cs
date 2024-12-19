using GensokyoSurvivors.interfaces;
using Godot;
using System;

public partial class GostBrain : Node, IBrain
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

    public Vector2 GetIntendedMovement()
    {
        // for now, behave like player.
		return Input.GetVector("move_left", "move_right", "move_up", "move_down").Normalized();
    }

}
