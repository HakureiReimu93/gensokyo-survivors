using GensokyoSurvivors.interfaces;
using Godot;

public partial class Unitmodel : CharacterBody2D
{
    private const int cAccelHuge = 500;
    [Export(PropertyHint.Range, "0,1")]
	float AccelDecelPercent;

	[Export(PropertyHint.Range, "20,800")]
	float MaximumSpeed;

	HealthTrait mHealth;
	IBrain mBrain;

	float mPercentageMaxSpeed;
	
	public override void _Ready()
    {
		var maybeHealth = GetNodeOrNull<HealthTrait>("HealthTrait");
		if (maybeHealth is null)
		{
			GD.Print("[ERROR], no 'HealthTrait' trait found!");
			QueueFree();
		}
		else
		{
			mHealth = maybeHealth;
		}
		var maybeBrain = GetNodeOrNull<IBrain>("Brain");
		if (maybeBrain is null)
		{
			GD.Print("[ERROR], no 'Brain' trait found! (IBrain)");
		}
		else
		{
			mBrain = maybeBrain;
		}
    }

    public override void _Process(double delta)
    {
		var movementDirection = mBrain.GetIntendedMovement();
		if (movementDirection.IsZeroApprox())
		{
			mPercentageMaxSpeed = Mathf.MoveToward(
                from:  mPercentageMaxSpeed,
                to:    0,
                delta: AccelDecelPercent * (float)delta * cAccelHuge);
		}
		else
		{
			mPercentageMaxSpeed = Mathf.MoveToward(
				from: mPercentageMaxSpeed,
				to: 	1,
				delta: AccelDecelPercent * (float)delta * cAccelHuge
			);
		}

		Velocity = movementDirection * MaximumSpeed * mPercentageMaxSpeed;

		MoveAndSlide();
    }
}
