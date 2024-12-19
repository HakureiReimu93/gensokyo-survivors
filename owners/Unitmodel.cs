using Godot;

public partial class Unitmodel : CharacterBody2D
{
	[Export(PropertyHint.Range, "0,1")]
	float AccelDecelPercent;

	
	HealthTrait mHealth;
	
	public override void _Ready()
    {
		var maybeHealth = GetNodeOrNull<HealthTrait>("Health");
		if (maybeHealth is null)
		{
			GD.Print("[ERROR], no health trait found!");
			QueueFree();
		}
    }

    public override void _Process(double delta)
    {

    }
}
