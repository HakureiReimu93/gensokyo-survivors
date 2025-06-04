using Godot;
using GodotStrict.Helpers.Guard;
using GodotStrict.Types.Lite;
using GensokyoSurvivors.Core.Interface;
using GodotStrict.AliasTypes;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/equation.png")]
public partial class AccelDecelMiddleware : Node, IScalarMiddleware<Vector2>
{
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

	public normal NextValue(Vector2 pValue, double delta)
	{
		SafeGuard.EnsureWithin(pValue.LengthSquared(), 0f, 1f, "Must normalize direction");

		if (pValue == Vector2.Zero)
		{
			data.ToZeroSpeed(delta);
		}
		else
		{
			data.Direction = pValue;
			data.ToTopSpeed(delta);
		}

		return data.Velocity.Length();
	}

	[Export(PropertyHint.Range, "0, 1")]
	float MyAccelFactor
	{
		get => myAccelFactor;
		set
		{
			myAccelFactor = value;
			data.Acceleration = value;
		}
	}

	[Export(PropertyHint.Range, "0, 1")]
	float MyDecelFactor
	{
		get => myDecelFactor;
		set
		{
			myDecelFactor = value;
			data.Deceleration = value;
		}
	}

	private float myDecelFactor;
	private float myMaximum;
	private float myAccelFactor;

	FlyweightVector2 data;
}
