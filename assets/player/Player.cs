using Godot;
using System;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

public partial class Player : CharacterBody2D
{
    public const int MAXSPEED = 200;

    public override void _Ready()
    {
        
    }

    // called each frame
    public override void _Process(double delta)
    {
        var movementVector = GetMovementVector();
        var direction = movementVector.Normalized();
        Velocity = direction * MAXSPEED;
        MoveAndSlide();
    }
//hey bub
    public Vector2 GetMovementVector()
    {
        var movementVector = Vector2.Zero;
        var xMovement = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        var yMovement = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");

        Vector2 vector = new Vector2(xMovement, yMovement);
        return vector;
    }



}