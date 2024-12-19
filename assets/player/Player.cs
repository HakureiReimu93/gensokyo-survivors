using Godot;
using System;

public partial class Player : CharacterBody2D
{
    public override void _Ready()
    {
        string fatFuck = "youBitch";
        GD.Print(fatFuck);

    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public override void get_movement_vector()
    {
        
    }

}