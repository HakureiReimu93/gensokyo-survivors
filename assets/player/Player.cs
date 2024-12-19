using Godot;
using System;

public partial class Player : CharacterBody2D
{

    //https://live.codetogether.io/#/10bceec1-c85e-4fbd-a744-e55c7a46bac9/8uDIKyDD7X7MBnFfUnTOer
    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
        var movementVector = GetMovementVector();
        var direction = movementVector.Normalized();
    }

    public Vector2 GetMovementVector()
    {
        var movementVector = Vector2.Zero;
        var xMovement = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        var yMovement = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");

        Vector2 vector = new Vector2(xMovement, yMovement);
        return vector;
    }

}