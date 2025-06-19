using System;
using GensokyoSurvivors.Source.Library.Common;
using Godot;
using GodotStrict.Helpers;
using GodotStrict.Helpers.Guard;
using GodotStrict.Types;

namespace GensokyoSurvivors.Src.Library.Buf;

[GlobalClass]
[Icon("res://GodotEditor/Icons/buf.png")]
public partial class TakeDamageBuf : UnitBuf
{
    public override BufStackType StackType => BufStackType.OnlyOne;

    public override StringName Identifier => cId;

    protected override void _Process(double delta)
    {
        mUnit.MyTempMovementSpeed *= Mathf.Lerp(
            1 - MyCrippleAmount,
            1f,
            Calculate.PhiEasing(0.35f, (mTimer.TimeLeft / mTimer.WaitTime).Norm10Invert())
        );

        if (mTimer.Tick(delta)) Invalidate();
    }

    protected override void _VisualProcess(double delta)
    {
        mUnit.MyModulateTemp = MyCrippleColor.Lerp(
            mUnit.MyModulateTemp,
            Calculate.PhiEasing(0.35f, (mTimer.TimeLeft / mTimer.WaitTime).Norm10Invert())
        );
    }

    public override void ParamInit(UnitModel unit, Godot.Collections.Array<UnitBuf> pParent)
    {
        base.ParamInit(unit, pParent);
        SafeGuard.Ensure(MyCrippleAmount > 0);
        SafeGuard.Ensure(MyCrippleTime > 0);
        mTimer.AutoRestart = false;
        mTimer.IsRunning = false;

        mTimer.WaitTime = MyCrippleTime;
        mTimer.TimeLeft = MyCrippleTime;
    }


    [Export(PropertyHint.Range, "0,1")]
    public float MyCrippleAmount { get; set; }

    [Export(PropertyHint.Range, "0,8")]
    public float MyCrippleTime
    {
        get
        {
            return myCrippleTime;
        }

        set
        {
            myCrippleTime = value;

            mTimer.WaitTime = value;
            mTimer.TimeLeft = value;
        }
    }

    [Export]
    public Color MyCrippleColor { get; set; } = Colors.DarkRed;

    private LiteTimer mTimer;
    private float myCrippleTime;
    readonly StringName cId = new("TakeDamageBuf");
}
