using GensokyoSurvivors.Source.Library.Common;
using Godot;
using GodotStrict.Helpers;
using GodotStrict.Helpers.Guard;
using GodotStrict.Types;
using System;

[GlobalClass]
[Icon("res://GodotEditor/Icons/output.png")]
public partial class HarmVisualMidWare : Node, IVisualMidWare
{
    public void NextEffect(CanvasItem input, double delta)
    {
        if (ProcessMode == ProcessModeEnum.Disabled) return;

        if (mTimer.Tick(delta))
        {
            ProcessMode = ProcessModeEnum.Disabled;
        }
        else
        {
            var percentLeft = Calculate.PhiEasing(0.7f, (1f - (mTimer.TimeLeft / mTimer.WaitTime)).Norm10());
            input.Modulate = MyHarmColor.Lerp(Colors.White, percentLeft);
        }
    }

    public void ActivateForSeconds(float duration = 0)
    {
        if (duration == 0)
        {
            mTimer.ResetWithCustomTime(1f);
        }
        else
        {
            mTimer.ResetWithCustomTime(duration);
        }
    }

    LiteTimer mTimer;

    [Export]
    Color MyHarmColor { get; set; } = Colors.DarkRed;

}
