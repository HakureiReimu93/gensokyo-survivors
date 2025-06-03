using Godot;
using GodotStrict.Helpers.Guard;
using GodotStrict.Types.Lite;
using GensokyoSurvivors.Core.Interface;
using GodotStrict.AliasTypes;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/equation.png")]
public partial class AccelDecelMiddleware : Node, IVelocityBuf
{
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
		SafeGuard.EnsureFloatsNotEqual(MyAccelFactor, 0, "Middleware user will not be able to move.");

		data = new FlyweightVector2
		{
			Acceleration = MyAccelFactor,
			Deceleration = MyDecelFactor,
			MaxSpeed = 1
		};
	}

	public Vector2 GetNextVelocity(Vector2 pDirection, double delta)
	{
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

	public Vector2 GetVelocityBuf(Vector2 pDirection, float baseSpeed, double delta)
	{
		SafeGuard.EnsureWithin(pDirection.LengthSquared(), 0f, 1f, "Must normalize direction");

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
  }
