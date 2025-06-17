using Godot;
using GensokyoSurvivors.Source.Library.Common;
using GodotStrict.Helpers.Guard;
using GodotStrict.Helpers;
using GodotStrict.Helpers.Logging;
using GodotStrict.AliasTypes;

[GlobalClass]
[Icon("res://GodotEditor/Icons/output.png")]
public partial class AccelDecelTrait : Node, IValueBuf<Vector2>
{

	public void CalculateValue(ref Vector2 input)
	{

		// curve that becomes infinity at 1.
		float accelMultiplier = Calculate.PhiEasing(0.5f, new normal(MyAcceleration)) * 30;
		float decelMultiplier = Calculate.PhiEasing(0.5f, new normal(MyDeceleration)) * 30;

		this.LogAny(decelMultiplier);

		double deltaTime = GetProcessDeltaTime();

		if (input.IsZeroApprox())
		{
			mCurrent = mCurrent.MoveToward(Vector2.Zero, (float)deltaTime * MyDeceleration * decelMultiplier);
		}
		else
		{
			mCurrent = mCurrent.MoveToward(input, (float)deltaTime * MyAcceleration * accelMultiplier);
		}

		input = mCurrent;

	}

	private Vector2 mCurrent;

	[ExportCategory("Set Speed in Unit")]
	[Export(PropertyHint.Range, "0,1")]
	public float MyAcceleration { get; set; }

	[Export(PropertyHint.Range, "0,1")]
	public float MyDeceleration { get; set; }
}
