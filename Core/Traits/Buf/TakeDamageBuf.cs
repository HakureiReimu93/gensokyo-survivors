using System;
using Godot;
using GodotStrict.AliasTypes;
using GodotStrict.Helpers.Guard;
using GodotStrict.Types;

[GlobalClass]
[Icon("res://Assets/GodotEditor/Icons/buf.png")]
public partial class TakeDamageBuf : UnitBuf
{
    public override float GetMovementSpeedScale()
    {
        return base.GetMovementSpeedScale() * 0.6f;
    }

    public override Color GetModulateColor()
    {
        var timeElapsedPercentage = mTimer.TimeLeft / mTimer.WaitTime;

        var modColor = mOptDamageExtremeness.MatchValue(
            some: (extremeness) => Colors.White.Lerp(MyBaseModulateColor, extremeness),
            none: () => MyBaseModulateColor
        );

        // power must be even to work properly
        // Ease-in slope U (x-1)^2
        return modColor.Lerp(Colors.White, Mathf.Pow(timeElapsedPercentage - 1, 2f));
    }

    public override bool IsExpired()
    {
        return mTimer.TimedOut;
    }

    public override void OnUnitProcess(double deltaTime)
    {
        base.OnUnitProcess(deltaTime);

        mTimer.Tick(deltaTime);
    }

    public void ParamInit(normal pDamageExtremeness)
    {
        mOptDamageExtremeness = Option<normal>.Ok(pDamageExtremeness);
    }

    public override void _Ready()
    {
        SafeGuard.Ensure(MyDuration != 0);
    }
	
    [Export]
    Color MyBaseModulateColor { get; set; } = Colors.White;

    [Export(PropertyHint.Range, "0,5")]
    float MyDuration
    {
        get
        {
            return mDuration;
        }
        set
        {
            mDuration = value;
            mTimer.ResetWithCustomTime(value);
        }
    }

    float mDuration;
    LiteTimer mTimer;
    Color mCurrentColor;
    Option<normal> mOptDamageExtremeness;

}