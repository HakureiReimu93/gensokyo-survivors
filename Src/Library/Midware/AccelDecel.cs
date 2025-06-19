using GensokyoSurvivors.Src.Library.Midware;
using Godot;
using GodotStrict.AliasTypes;
using GodotStrict.Helpers;
using System;

[GlobalClass]
[Icon("res://GodotEditor/Icons/equation.png")]
public partial class AccelDecel : UnitMidWare
{
    public override void Process(UnitModel input, double delta)
    {
        float accelMultiplier = Calculate.PhiEasing(0.5f, new normal(MyAcceleration)) * 30;
        float decelMultiplier = Calculate.PhiEasing(0.5f, new normal(MyDeceleration)) * 30;

        if (input.MyPlannedMoveDirection.IsZeroApprox())
        {
            mCurrent = mCurrent.MoveToward(Vector2.Zero, (float)delta * MyDeceleration * decelMultiplier);
        }
        else
        {
            mCurrent = mCurrent.MoveToward(input.MyPlannedMoveDirection, (float)delta * MyAcceleration * accelMultiplier);
        }

        input.MyPlannedMoveDirection = mCurrent;
    }

    private Vector2 mCurrent;

    [Export(PropertyHint.Range, "0,1")]
    public float MyAcceleration { get; set; }

    [Export(PropertyHint.Range, "0,1")]
    public float MyDeceleration { get; set; }
}
