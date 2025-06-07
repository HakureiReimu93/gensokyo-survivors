using Godot;
using System;


// TODO:
// set up a way for this time display to have its time set.
// i.e. warrants the creation of a chain scanner
// use groups to listen to time updates
// from i.e.: chan-arena-time.

// UI's are not allowed to 'poke' at other objects involuntarily.
// Everything must come to It.
// i.e. the ArenaSession should talk to the time display.
// first, it gets chan-arena-time members
// then, every second, will call: SendCurrentTimeValue() on the chain scanner

// chain scanners are for when one class needs to talk to many distant receivers.
// regular scanners are for many-to-one relationships.
public partial class TimeDisplay : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
