using Godot;
using GodotStrict.AliasTypes;
using GodotUtilities;
using System;

[UseAutowiring]
public partial class HealthBar : Control
{
    [Autowired]
    ProgressBar mProgress;

    public override void _Ready()
    {
        __PerformDependencyInjection();
    }

    public void UpdatePercentage(normal percentage)
    {
        if (percentage != mProgress.Value)
        {
            mUpdatePercentageTween?.Kill();

            mUpdatePercentageTween = GetTree().CreateTween();
            mUpdatePercentageTween
                .TweenMethod(Callable.From<float>(_UpdatePercentage), mProgress.Value, (float)percentage * 100f, 0.3f)
                .SetTrans(Tween.TransitionType.Quad);
        }
    }

    private void _UpdatePercentage(float percent)
    {
        mProgress.Value = percent;
    }

    Tween mUpdatePercentageTween;
}
