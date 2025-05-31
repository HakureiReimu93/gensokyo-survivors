using Godot;
using static GodotStrict.Helpers.Guard.SafeGuard;
using GodotStrict.Types.Lite;
using GensokyoSurvivors.Core.Interface;

[GlobalClass]
[Icon("res://Assets/Icons/equation.png")]
public partial class SmoothVelocityMiddleware : Node, IVelocityMiddleware
{
	[Export(PropertyHint.Range, "0, 900")]
    float MyMaximum
    {
        get => _myMaximum;
        set
        {
            _myMaximum = value;
            data.MaxSpeed = value;
        }
    }
    private float _myMaximum;

    [Export(PropertyHint.Range, "0, 1")]
    float MyAccelFactor
    {
        get => _myAccelFactor;
        set
        {
            _myAccelFactor = value;
            data.Acceleration = value;
        }
    }
    private float _myAccelFactor;

    [Export(PropertyHint.Range, "0, 1")]
    float MyDecelFactor
    {
        get => _myDecelFactor;
        set
        {
            _myDecelFactor = value;
            data.Deceleration = value;
        }
    }
    private float _myDecelFactor;

	FlyweightVector2 data;

	public override void _Ready()
	{
		EnsureFloatsNotEqual(MyMaximum, 0, "Middleware user will not be able to move.");
		EnsureFloatsNotEqual(MyAccelFactor, 0, "Middleware user will not be able to move.");
		
		data = new FlyweightVector2
		{
			Acceleration = MyAccelFactor,
			Deceleration = MyDecelFactor,
			MaxSpeed = MyMaximum
		};
	}

	public Vector2 GetNextVelocity(Vector2 pDirection, double delta)
	{
		EnsureThatFloat(pDirection.LengthSquared())
			.CanBe(1)
			.CanBe(0)
			.ButNothingElseBecause("Set speed manually with DoSetSpeedForcely()");

		if (pDirection == Vector2.Zero)
		{
			data.ToZeroSpeed(delta);
		}
		else
		{
			data.Direction = pDirection;
			data.ToTopSpeed(delta);
		}

		return data.Velocity;
	}

	public void DoSetSpeedForcely(float pSpeed)
	{
		data.MaxSpeed = pSpeed;
	}
}
