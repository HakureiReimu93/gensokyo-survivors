using Godot;
using System;

public partial class Player : CharacterBody2D
{

    //https://live.codetogether.io/#/10bceec1-c85e-4fbd-a744-e55c7a46bac9/8uDIKyDD7X7MBnFfUnTOer
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